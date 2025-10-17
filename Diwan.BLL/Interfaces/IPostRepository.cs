using Diwan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Interfaces
{
    public interface IPostRepository : IGenericRepository<Post>
    {
        Task<IEnumerable<Post>> GetFriendsPostsAsync(string id);
        Task<IEnumerable<Post>> GetUserPostsAsync(string id);
    }
}
