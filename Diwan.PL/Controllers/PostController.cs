using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
