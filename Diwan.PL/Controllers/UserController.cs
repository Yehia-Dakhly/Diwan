using AutoMapper;
using Azure.Core;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Policy;
using System.Threading.Tasks;

namespace Diwan.PL.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<DiwanUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserController(UserManager<DiwanUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<IActionResult> Profile(string id)
        {
            var CurrentUser = _userManager.GetUserId(User);
            var UserProfile = await _unitOfWork.UserRepository.FindFirstAsync(U => U.Id == id, includes: [U => U.Posts]);
            if (UserProfile is not null)
            {
                var Posts = UserProfile.Posts.ToList();
                var MappedPosts = new List<PostViewModel>();
                    
                if (Posts is not null)
                {
                    MappedPosts = _mapper.Map<IEnumerable<Post>, IEnumerable<PostViewModel>>(Posts).ToList();
                }

                var FriendStatus = FriendRequestStatus.Unknown;
                var Friendship = await _unitOfWork.FriendshipRepository.FindFirstAsync(F => (F.AddresseeId == id && F.RequesterId == CurrentUser) || (F.AddresseeId == CurrentUser && F.RequesterId == id));
                if (Friendship is not null)
                {
                    FriendStatus = Friendship.Status;
                }

                var MappedProfile = new UserProfileViewModel()
                {
                    FullName = $"{UserProfile.FirstName} {UserProfile.LastName}",
                    Id = id,
                    Bio = UserProfile.Bio,
                    CoverPicURL = UserProfile.CoverPicURL,
                    DateOfBirth = UserProfile.DateOfBirth,
                    Gender = UserProfile.Gender,
                    PictureURL = UserProfile.PictureURL,

                    IsFriendshipInitiator = Friendship is not null && Friendship.RequesterId == _userManager.GetUserId(User),
                    FriendshipStatus = FriendStatus,
                    IsCurrentUserProfile = id == _userManager.GetUserId(User),
                    Posts = MappedPosts
                };
                return View(MappedProfile);
            }
            return RedirectToAction("Index", "Home");
        }
        
    }
}
