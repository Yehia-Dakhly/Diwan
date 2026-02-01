using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Interfaces
{
    public interface IUnitOfWork
    {
        public IPostRepository PostRepository { get; set; }
        public IUserRepository UserRepository { get; set; }
        public IFriendshipRepository FriendshipRepository { get; set; }
        public INotificationRepository NotificationRepository { get; set; }
        public ICommentRepository CommentRepository { get; set; }
        public IReactionRepository ReactionRepository { get; set; }
        Task<int> CompleteAsync();
    }
}
