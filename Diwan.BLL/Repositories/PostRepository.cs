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
    public class PostRepository : GenericRepository<Post>, IPostRepository
    {
        private readonly DiwanDbContext _diwanDbContext;

        public PostRepository(DiwanDbContext diwanDbContext) : base(diwanDbContext)
        {
            _diwanDbContext = diwanDbContext;
        }

        public async Task<IEnumerable<Post>> GetFriendsPostsAsync(string id)
        {
            var Friends = await _diwanDbContext.Friendships
                .Where(
                        F => F.Status == DAL.Enums.FriendRequestStatus.Accepted
                         && (F.RequesterId == id || F.AddresseeId == id)
                ).Select(F => F.RequesterId == id ? F.AddresseeId : F.RequesterId).ToListAsync();

            var Posts = await _diwanDbContext.Posts.Where(P => (P.Visibility != DAL.Enums.Visibility.Private) && Friends.Contains(P.AuthorId) || P.AuthorId == id || P.Visibility == DAL.Enums.Visibility.Public)
                .Include(P => P.Author).Include(P => P.Reactions).Include(P => P.Comments).OrderByDescending(P => P.CreatedAt).ToListAsync();

            return Posts;
        }
    }
}
