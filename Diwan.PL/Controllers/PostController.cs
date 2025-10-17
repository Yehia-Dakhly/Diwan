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
    public class PostController : Controller
    {
        private readonly UserManager<DiwanUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PostController(UserManager<DiwanUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IActionResult CreatePost()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreatePost(CreatePostViewModel Post)
        {
            if (ModelState.IsValid)
            {
                var MappedPost = _mapper.Map<Post>(Post);
                var Current = _userManager.GetUserId(User);
                if (Current is null)
                    return RedirectToAction("Login", "Account");

                if (Post.Picture is not null)
                    MappedPost.PictureURL = Helpers.DocumentSettings.UploadFile(Post.Picture, "Posts");
                MappedPost.AuthorId = Current;
                if (Post.Visibility == DAL.Enums.Visibility.Public || Post.Visibility == DAL.Enums.Visibility.Friends)
                {
                    var CurrentUser = await _userManager.GetUserAsync(User);
                    if (CurrentUser is null)
                    {
                        return RedirectToAction("Login", "Account");
                    }
                    var Friends = await _unitOfWork.FriendshipRepository.GetFirendsIDsAsync(CurrentUser.Id);
                    foreach (var item in Friends)
                    {
                        var NewNotification = new Notification()
                        {
                            ActorUserId = CurrentUser.Id,
                            Message = $"{CurrentUser.FirstName} {CurrentUser.LastName} Has Shared A New Post!",
                            IsRead = false,
                            RecipientUserId = item,
                            NotificationType = DAL.Enums.NotificationType.NewSharedPost,
                            URL = Url.Action("PostDetails", "Post", new { id = Post.Id })
                        };
                        await _unitOfWork.NotificationRepository.AddAsync(NewNotification);
                    }
                }

                await _unitOfWork.PostRepository.AddAsync(MappedPost);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Profile", "Profile", new { id = Current });
            }
            return View(Post);
        }
        public async Task<IActionResult> PostDetails(int id)
        {
            var Post = await _unitOfWork.PostRepository.FindFirstAsync(P => P.Id == id, includes: [P => P.Comments, P => P.Reactions, P => P.Author]);
            if (Post is not null)
            {
                var MapppedPost = _mapper.Map<PostViewModel>(Post);
                return View(MapppedPost);
            }
            return NotFound();
        }
        public async Task<IActionResult> EditPost([FromRoute] int id)
        {
            var CurrentUser = _userManager.GetUserId(User);
            var Post = await _unitOfWork.PostRepository.FindFirstAsync(P => P.Id == id);
            if (Post is not null && CurrentUser == Post.AuthorId)
            {
                var MappedPost = _mapper.Map<EditPostViewModel>(Post);
                return View(MappedPost);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditPost([FromRoute] int id, EditPostViewModel model)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = _userManager.GetUserId(User);
                if (CurrentUser == model.AuthorId && id == model.Id)
                {
                    var Post = await _unitOfWork.PostRepository.FindFirstAsync(P => P.Id == model.Id);
                    if (Post is not null)
                    {
                        _mapper.Map(model, Post);
                        if (model.Picture is not null)
                        {
                            Post.PictureURL = Helpers.DocumentSettings.UploadFile(model.Picture, "Posts");
                        }
                        Post.UpdatedAt = DateTime.Now;
                        _unitOfWork.PostRepository.Update(Post);
                        await _unitOfWork.CompleteAsync();
                        return RedirectToAction("PostDetails", "Post", new { id = model.Id });
                    }
                }
                return BadRequest();
            }
            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> DeletePost([FromRoute] int id)
        {
            var CurrentUser = _userManager.GetUserId(User);
            var Post = await _unitOfWork.PostRepository.FindFirstAsync(P => P.Id == id, includes: [P => P.Comments, P => P.Reactions]);
            if (Post is not null && Post.AuthorId == CurrentUser)
            {
                var Reactions = await _unitOfWork.ReactionRepository.FindAsync(R => R.PostId == id);
                foreach (var item in Reactions)
                {
                    _unitOfWork.ReactionRepository.Delete(item);
                }
                var Comments = await _unitOfWork.CommentRepository.FindAsync(P => P.PostId == id);
                Comments = Comments.OrderByDescending(C => C.Id).ToList();
                foreach (var item in Comments)
                {
                    _unitOfWork.CommentRepository.Delete(item);
                }
                _unitOfWork.PostRepository.Delete(Post);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Profile", "Profile", new { id = Post.AuthorId });
            }
            return BadRequest();
        }
        public async Task<IActionResult> ToggleReaction([FromBody] ReationViewModel model)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(currentUserId))
            {
                return Unauthorized();
            }

            var existingReaction = await _unitOfWork.ReactionRepository.FindFirstAsync(
                r => r.PostId == model.PostId && r.UserId == currentUserId
            );

            if (existingReaction != null)
            {
                if (existingReaction.ReactionType == model.ReactionType)
                {
                    _unitOfWork.ReactionRepository.Delete(existingReaction);
                }
                else
                {
                    existingReaction.ReactionType = model.ReactionType;
                    _unitOfWork.ReactionRepository.Update(existingReaction);
                }
            }
            else
            {
                var newReaction = new Reaction
                {
                    PostId = model.PostId,
                    UserId = currentUserId,
                    ReactionType = model.ReactionType
                };
                await _unitOfWork.ReactionRepository.AddAsync(newReaction);
            }

            await _unitOfWork.CompleteAsync();

            var allReactionsForPost = await _unitOfWork.ReactionRepository.FindAsync(r => r.PostId == model.PostId);

            var countsByType = allReactionsForPost
                .GroupBy(r => r.ReactionType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            return Json(new { countByType = countsByType });
        }

    }
}
