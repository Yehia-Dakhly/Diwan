using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Diwan.PL.Controllers
{
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
                await _unitOfWork.PostRepository.AddAsync(MappedPost);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Index", "Home");
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
                return RedirectToAction("Profile", "User", new {id = Post.AuthorId});
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

            // 1. Check if a reaction from this user already exists on this post
            var existingReaction = await _unitOfWork.ReactionRepository.FindFirstAsync(
                r => r.PostId == model.PostId && r.UserId == currentUserId
            );

            // Case 1: A reaction already exists
            if (existingReaction != null)
            {
                //var Notification = _unitOfWork.NotificationRepository.FindFirstAsync(N => N.);
                // If the user clicked the SAME reaction again, they are "un-reacting"
                if (existingReaction.ReactionType == model.ReactionType)
                {
                    _unitOfWork.ReactionRepository.Delete(existingReaction);
                }
                // If the user clicked a DIFFERENT reaction, they are changing their mind
                else
                {
                    existingReaction.ReactionType = model.ReactionType;
                    _unitOfWork.ReactionRepository.Update(existingReaction);
                }
            }
            // Case 2: No reaction exists yet
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

            // Save the change (create, update, or delete) to the database
            await _unitOfWork.CompleteAsync();

            // 2. After saving, get the new counts to send back to the UI
            var allReactionsForPost = await _unitOfWork.ReactionRepository.FindAsync(r => r.PostId == model.PostId);

            var countsByType = allReactionsForPost
                .GroupBy(r => r.ReactionType)
                .ToDictionary(g => g.Key.ToString(), g => g.Count());

            // 3. Return a JSON object with the updated counts
            return Ok(new { totalCount = allReactionsForPost.Count(), countsByType });
        }

    }
}
