using Diwan.BLL.Interfaces;
using Diwan.DAL.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diwan.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DiwanDbContext _diwanDbContext;

        public IUserRepository UserRepository {  get; set; }
        public IPostRepository PostRepository { get; set; }
        public IFriendshipRepository FriendshipRepository { get; set; }
        public INotificationRepository NotificationRepository { get; set; }
        public ICommentRepository CommentRepository { get; set; }
        public IReactionRepository ReactionRepository { get; set; }
        public UnitOfWork(DiwanDbContext diwanDbContext, IPostRepository postRepository,
            IFriendshipRepository friendshipRepository, INotificationRepository notificationRepository,
            ICommentRepository commentRepository, IReactionRepository reactionRepository, IUserRepository userRepository) // ASK CLR For Inject dbContext
        {
            _diwanDbContext = diwanDbContext;   
            PostRepository = postRepository ;
            FriendshipRepository = friendshipRepository;
            NotificationRepository = notificationRepository;
            CommentRepository = commentRepository;
            ReactionRepository = reactionRepository;
            UserRepository = userRepository;
        }
        public async Task<int> CompleteAsync()
        {
            return await _diwanDbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _diwanDbContext.Dispose();
        }
    }
}
