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
                return RedirectToAction("Index");
            }
            return View(Post);
        }
    }
}
