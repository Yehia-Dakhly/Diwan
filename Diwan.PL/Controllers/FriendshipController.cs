using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Enums;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Diwan.PL.Controllers
{
    public class FriendshipController : Controller
    {
        private readonly UserManager<DiwanUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FriendshipController(UserManager<DiwanUser> userManager, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [HttpGet]
        public async Task<IActionResult> AddFriend(string UserName)
        {
            var currentUserId = _userManager.GetUserId(User);
            if (UserName is not null)
            {
                var Friends = await _unitOfWork.UserRepository.FindFriendsAsync(UserName);
                if (Friends == null)
                    return View();

                //var MappedFriends = _mapper.Map<IEnumerable<DiwanUser>, IEnumerable<DiwanUserViewModel>>(Friends);
                var MappedFriends = new List<DiwanUserViewModel>();
                var MyFriendships = await _unitOfWork.FriendshipRepository.FindAsync(F => F.AddresseeId == currentUserId || F.RequesterId == currentUserId);
                foreach (var user in Friends)
                {
                    if (user.Id == currentUserId)
                        continue;
                    var viewModel = _mapper.Map<DiwanUserViewModel>(user);

                    // Find if a Friendship exists with this specific user
                    var Friendship = MyFriendships.FirstOrDefault(
                        F => (F.RequesterId == currentUserId && F.AddresseeId == user.Id)
                        || (F.RequesterId == user.Id && F.AddresseeId == currentUserId)
                    );
                    if (Friendship != null)
                    {
                        viewModel.FriendshipStatus = Friendship.Status;
                        viewModel.IsFriendshipInitiator = (Friendship.RequesterId == currentUserId);
                    }
                    MappedFriends.Add(viewModel);
                }
                return View(MappedFriends);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> RequestFriend(string addresseeId)
        {
            var RequeserID = await _userManager.GetUserAsync(User);
            if (RequeserID is null)
            {
                return RedirectToAction("Login", "Account");
            }
            var Found = await _unitOfWork.FriendshipRepository.FindFirstAsync(F => (F.AddresseeId == addresseeId && F.RequesterId == RequeserID.Id) || (F.AddresseeId == RequeserID.Id && F.RequesterId == addresseeId));
            if (Found is null)
            {
                var NewFriendship = new Friendship() { AddresseeId = addresseeId, RequesterId = RequeserID.Id, Status = DAL.Enums.FriendRequestStatus.Pending };
                await _unitOfWork.FriendshipRepository.AddAsync(NewFriendship);
            }
            else
            {
                Found.Status = DAL.Enums.FriendRequestStatus.Pending;
                _unitOfWork.FriendshipRepository.Update(Found);
            }
            var NewNotification = new Notification()
            {
                ActorUserId = RequeserID.Id,
                Message = $"{RequeserID.UserName} Has Send You A Friend Request!",
                IsRead = false,
                RecipientUserId = addresseeId,
                NotificationType = DAL.Enums.NotificationType.NewFriendRequest,
                URL = Url.Action("FriendRequests", "Friendship")
            };
            await _unitOfWork.NotificationRepository.AddAsync(NewNotification);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("Index", "Home");
        }
        public async Task<IActionResult> FriendRequests()
        {
            var UserId = _userManager.GetUserId(User);
            if (UserId is not null)
            {
                var Requested = await _unitOfWork.FriendshipRepository.FindAsync(F => F.AddresseeId == UserId && F.Status == DAL.Enums.FriendRequestStatus.Pending, includes: [F => F.Requester]);
                var MappedFriendships = _mapper.Map<IEnumerable<Friendship>, IEnumerable<FriendshipViewModel>>(Requested);
                return View(MappedFriendships);
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AcceptRequest(int friendshipId)
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null) {
                return RedirectToAction("Index", "Home");
            }
            var Friendship = await _unitOfWork.FriendshipRepository.FindFirstAsync(F => F.Id == friendshipId, includes: [F => F.Addressee]);
            if (Friendship is not null)
            {
                Friendship.Status = DAL.Enums.FriendRequestStatus.Accepted;
                _unitOfWork.FriendshipRepository.Update(Friendship);
                var NewNotification = new Notification()
                {
                    ActorUserId = CurrentUser,
                    Message = $"{Friendship.Addressee.FirstName} {Friendship.Addressee.FirstName} Has Accecpted Your Friend Request!",
                    IsRead = false,
                    RecipientUserId = Friendship.RequesterId,
                    NotificationType = DAL.Enums.NotificationType.FriendRequestAccepted,
                    URL = Url.Action("Profile", "User", new { id = Friendship.AddresseeId })
                };
                await _unitOfWork.NotificationRepository.AddAsync(NewNotification);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(FriendRequests));
        }
        public async Task<IActionResult> DeclineRequest(int friendshipId)
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Index", "Home");
            }
            var Friendship = await _unitOfWork.FriendshipRepository.FindFirstAsync(F => F.Id == friendshipId);
            if (Friendship is not null)
            {
                Friendship.Status = DAL.Enums.FriendRequestStatus.Declined;
                _unitOfWork.FriendshipRepository.Update(Friendship);
                await _unitOfWork.CompleteAsync();
            }

            return RedirectToAction(nameof(FriendRequests));
        }

        public async Task<IActionResult> Friends()
        {
            var CurrentUserId = _userManager.GetUserId(User);
            if (CurrentUserId == null)
                return RedirectToAction("Login", "Account");

            var Friendships = await _unitOfWork.FriendshipRepository.FindAsync(F => (F.AddresseeId == CurrentUserId || F.RequesterId == CurrentUserId) && F.Status == FriendRequestStatus.Accepted, includes: [F => F.Addressee, F => F.Requester]);
            if (Friendships is not null)
            {
                var Friends = Friendships.Select(F => (F.RequesterId == CurrentUserId ? new FriendViewModel() { Id = F.AddresseeId, FullName = $"{F.Addressee.FirstName} {F.Addressee.LastName}", PictureURL = F.Addressee.PictureURL }
                : new FriendViewModel() { Id = F.RequesterId, FullName = $"{F.Requester.FirstName} {F.Requester.LastName}", PictureURL = F.Requester.PictureURL })).ToList();
                return View(Friends);
            }
                return View();
        }

        public async Task<IActionResult> DeleteFriend([FromRoute] string id)
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Login", "Account");
            }
            var Friendship = await _unitOfWork.FriendshipRepository.FindFirstAsync(F => (F.AddresseeId == id && F.RequesterId == CurrentUser) || F.AddresseeId == CurrentUser && F.RequesterId == id);
            if (Friendship is not null)
            {
                Friendship.Status = FriendRequestStatus.Unknown;
                _unitOfWork.FriendshipRepository.Update(Friendship);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Friends));
        }
        public async Task<IActionResult> BlockFriend([FromRoute] string id)
        {
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Login", "Account");
            }
            var Friendship = await _unitOfWork.FriendshipRepository.FindFirstAsync(F => (F.AddresseeId == id && F.RequesterId == CurrentUser) || F.AddresseeId == CurrentUser && F.RequesterId == id);
            if (Friendship is not null)
            {
                Friendship.Status = FriendRequestStatus.Blocked;
                _unitOfWork.FriendshipRepository.Update(Friendship);
                await _unitOfWork.CompleteAsync();
            }
            return RedirectToAction(nameof(Friends));
        }
    }
}
