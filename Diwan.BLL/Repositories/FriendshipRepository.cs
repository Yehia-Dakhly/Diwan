using Diwan.BLL.Interfaces;
using Diwan.DAL.Contexts;
using Diwan.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Repositories
{
    public class FriendshipRepository : GenericRepository<Friendship>, IFriendshipRepository
    {
        private readonly DiwanDbContext _diwanDbContext;

        public FriendshipRepository(DiwanDbContext diwanDbContext) : base(diwanDbContext)
        {
            _diwanDbContext = diwanDbContext;
        }

        public async Task<IEnumerable<string>> GetFirendsIDsAsync(string id)
        {
            var Friends = _diwanDbContext.Friendships.Where(F => F.Status == DAL.Enums.FriendRequestStatus.Accepted
            && (F.AddresseeId == id|| F.RequesterId == id)).Select(U => (U.AddresseeId == id ? U.RequesterId : U.AddresseeId));
            return await Friends.ToListAsync();
        }
    }
}
