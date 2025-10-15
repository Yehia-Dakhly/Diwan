using Diwan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Interfaces
{
    public interface IFriendshipRepository : IGenericRepository<Friendship>
    {
        Task<IEnumerable<string>> GetFirendsIDsAsync(string id);
    }
}
