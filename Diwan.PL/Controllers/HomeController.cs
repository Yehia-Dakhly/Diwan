using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Models;
using Diwan.PL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
//+ Fix Request Friend From Profile
//+ Fix View Reations at Profile
//+ Ensure Comments Number Are Viewed
//+ View Photos when I click it
//+ Add Notification Alert
//+ Read Notification
//+ Add \n in comments
//+ Hide The Menu of Edit and delete Post if I not the project Author
//+ Add Default Photo
// Reaction Appears After Click
// Remove Notification if the Posts Deleted
// Loading Posts && Notifications 5 by 5
// Dark Mode
// Friends Suggestions
// Authoritize [Admin] -> View Users
// -> Private Account
// -> Groups
namespace Diwan.PL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<DiwanUser> _userManager;
        private readonly IMapper _mapper;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork, UserManager<DiwanUser> userManager, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string? SearchValue)
        {
            if (SearchValue is null)
            {

                var Current = _userManager.GetUserId(User);
                if (Current is null)
                {
                    return RedirectToAction("Login", "Account");
                }
                
                var HaveNotification = await _unitOfWork.NotificationRepository.FindFirstAsync(N => N.RecipientUserId == Current && N.IsRead == false);
                if (HaveNotification is not null)
                    ViewData["HaveNotifications"] = true;
                else
                    ViewData["HaveNotifications"] = false;

                var Posts = await _unitOfWork.PostRepository.GetFriendsPostsAsync(Current);
                var MappedPosts = _mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(Posts);
                return View(MappedPosts);
            }
            else
            {

            }
            return View();
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
