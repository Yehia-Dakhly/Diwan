using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diwan.PL.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<DiwanUser> _userManager;
        private readonly IMapper _mapper;

        public CommentController(IUnitOfWork unitOfWork, UserManager<DiwanUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        public IActionResult CreateComment(int id)
        {
            var model = new CommentCreateViewModel { PostId = id };
            return PartialView("~/Views/Comment/PartialView/_CommentForm.cshtml", model);
        }
        [HttpPost]
        public async Task<IActionResult> CreateComment(CommentCreateViewModel model)
        {
            var CurrentUser = await _userManager.GetUserAsync(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Login", "Account");
            }
            var Comment = new Comment()
            {
                Content = model.Content,
                UserId = CurrentUser.Id,
                PostId = model.PostId,
            };
            var Post = await _unitOfWork.PostRepository.FindFirstAsync(P => P.Id == model.PostId);
            if (Post.AuthorId != CurrentUser.Id)
            {
                var NewNotification = new Notification()
                {
                    ActorUserId = CurrentUser.Id,
                    Message = $"{CurrentUser.FirstName} {CurrentUser.LastName} Has Commented On Your Post!",
                    IsRead = false,
                    RecipientUserId = Post.AuthorId,
                    NotificationType = DAL.Enums.NotificationType.NewPostComment,
                    URL = Url.Action("PostDetails", "Post", new { id = model.PostId })
                };
                await _unitOfWork.NotificationRepository.AddAsync(NewNotification);
            }
            await _unitOfWork.CommentRepository.AddAsync(Comment);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("PostDetails", "Post", new { id = model.PostId });
        }
        public async Task<IActionResult> _GetCommentsPartial(int postId)
        {
            try
            {
                var AllComments = await _unitOfWork.CommentRepository.FindAsync(
                    C => C.PostId == postId,
                    includes: [C => C.User]
                );

                var allCommentViewModels = _mapper.Map<IEnumerable<CommentViewModel>>(AllComments);

                var commentLookup = allCommentViewModels.ToDictionary(c => c.Id);
                var topLevelComments = new List<CommentViewModel>();

                foreach (var comment in allCommentViewModels)
                {
                    if (comment.ParentId != null && commentLookup.ContainsKey(comment.ParentId.Value))
                    {
                        commentLookup[comment.ParentId.Value].Replies.Add(comment);
                    }
                    else
                    {
                        topLevelComments.Add(comment);
                    }
                }

                var sortedComments = topLevelComments.OrderBy(c => c.CreatedAt).ToList();

                return PartialView("_CommentList", sortedComments);
            }
            catch (Exception ex)
            {
                return Content($"Error {ex.Message}");
            }
        }
        [HttpGet]
        public IActionResult ReplyComment(int postId, int parentId)
        {
            var viewModel = new ReplyCommentViewModel { PostId = postId, ParentId = parentId };
            return PartialView("_ReplyForm", viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ReplyComment(ReplyCommentViewModel model)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = await _userManager.GetUserAsync(User);
                var reply = new Comment
                {
                    Content = model.Content,
                    PostId = model.PostId,
                    ParentId = model.ParentId,
                    UserId = CurrentUser.Id,
                };
                var Parent = await _unitOfWork.PostRepository.FindFirstAsync(P => P.Id == model.PostId, includes: [P => P.Comments]);
                var ParentCommentUserId = Parent.Comments.FirstOrDefault(C => C.Id == model.ParentId);
                if (ParentCommentUserId is not null && ParentCommentUserId.UserId != CurrentUser.Id)
                {
                    var NewNotification = new Notification()
                    {
                        ActorUserId = CurrentUser.Id,
                        Message = $"{CurrentUser.FirstName} {CurrentUser.LastName} Replied To Your Comment!",
                        IsRead = false,
                        RecipientUserId = ParentCommentUserId.UserId,
                        NotificationType = DAL.Enums.NotificationType.NewCommentReply,
                        URL = Url.Action("PostDetails", "Post", new { id = model.PostId })
                    };
                    await _unitOfWork.NotificationRepository.AddAsync(NewNotification);
                }

                if (Parent.AuthorId != CurrentUser.Id)
                {
                    var PostUserNotification = new Notification()
                    {
                        ActorUserId = CurrentUser.Id,
                        Message = $"{CurrentUser.FirstName} {CurrentUser.LastName} Commented On Your Post!",
                        IsRead = false,
                        RecipientUserId = Parent.AuthorId,
                        NotificationType = DAL.Enums.NotificationType.NewPostComment,
                        URL = Url.Action("PostDetails", "Post", new { id = model.PostId })
                    };
                    await _unitOfWork.NotificationRepository.AddAsync(PostUserNotification);
                }
                await _unitOfWork.CommentRepository.AddAsync(reply);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("PostDetails", "Post", new { id = Parent.Id });
            }
            return BadRequest("Invalid reply data.");
        }
        public async Task<IActionResult> Delete(int id)
        {
            var UserId = _userManager.GetUserId(User);

            if (await _unitOfWork.CommentRepository.FindFirstAsync(C => C.UserId == UserId) != null)
            {
                int Temp = id;
                List<Comment> V = new List<Comment>();
                var Var = await _unitOfWork.CommentRepository.FindFirstAsync(F => F.Id == id);
                if (Var is not null)
                    V.Add(Var);
                var New = await _unitOfWork.CommentRepository.FindFirstAsync(C => C.ParentId == Temp);
                while (New != null)
                {
                    V.Add(New);
                    Temp = New.Id;
                    New = await _unitOfWork.CommentRepository.FindFirstAsync(C => C.ParentId == Temp);
                }
                for (int i = V.Count - 1; i >= 0; i--)
                {
                    _unitOfWork.CommentRepository.Delete(V[i]);
                }
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction("Index", "Home");
        }
    }
}
