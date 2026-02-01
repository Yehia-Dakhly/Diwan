using Diwan.BLL.Interfaces;
using Diwan.DAL.Contexts;
using Diwan.DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Repositories
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(DiwanDbContext diwanDbContext) : base(diwanDbContext)
        {
        }
    }
}
