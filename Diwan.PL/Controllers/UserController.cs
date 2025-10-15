using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diwan.PL.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Profile([FromRoute] string id)
        {

            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (id is null)
            {
                id = CurrentUser;
            }
            var UserProfile = await _unitOfWork.UserRepository.FindFirstAsync(U => U.Id == id, includes: [U => U.Posts, U => U.Reactions, User => User.Comments]);
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
                    IsCurrentUserProfile = (id == _userManager.GetUserId(User)),
                    Posts = MappedPosts
                };
                return View(MappedProfile);
            }
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> EditProfile([FromRoute] string id)
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser != id)
            {
                return BadRequest();
            }
            var UserProfile = await _unitOfWork.UserRepository.FindFirstAsync(U => U.Id == id);
            if (UserProfile is not null)
            {
                var MappedProfile = _mapper.Map<EditProfileViewModel>(UserProfile);
                return View(MappedProfile);
            }
            return BadRequest();
        }
        [HttpPost]
        public async Task<IActionResult> EditProfile(EditProfileViewModel model)
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser != model.Id)
            {
                return BadRequest();
            }
            var UserProfile = await _unitOfWork.UserRepository.FindFirstAsync(U => U.Id == model.Id);
            if (UserProfile is not null)
            {
                if (model.Picture != null)
                    UserProfile.PictureURL = Helpers.DocumentSettings.UploadFile(model.Picture, "Images");
                if (model.CoverPic != null)
                    UserProfile.CoverPicURL = Helpers.DocumentSettings.UploadFile(model.CoverPic, "Images");
                _mapper.Map(model, UserProfile);
                _unitOfWork.UserRepository.Update(UserProfile);
                await _unitOfWork.CompleteAsync();
                return RedirectToAction("Profile", "User", new { id = model.Id });
            }
            return BadRequest();
        }
        public async Task<IActionResult> Notification()
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Login", "Account");
            }
            var Notifications = (await _unitOfWork.NotificationRepository.FindAsync(N => N.RecipientUserId == CurrentUser, includes: [N => N.ActorUser])).ToList();
            var MappedNotifications = _mapper.Map<IEnumerable<NotificationViewModel>>(Notifications);
            return PartialView("~/Views/Notification/PartialView/_NotificationList.cshtml", MappedNotifications);
        }
    }
}
