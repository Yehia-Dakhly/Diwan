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
    public class UserRepository : GenericRepository<DiwanUser>, IUserRepository
    {
        private readonly DiwanDbContext _diwanDbContext;

        public UserRepository(DiwanDbContext diwanDbContext) : base(diwanDbContext)
        {
            _diwanDbContext = diwanDbContext;
        }

        public async Task<IEnumerable<DiwanUser>> FindFriendsAsync(string DiwanUserName)
        {
            return await _diwanDbContext.Users.Where(U => U.UserName.ToLower().Contains(DiwanUserName)).ToListAsync();
        }
    }
}
