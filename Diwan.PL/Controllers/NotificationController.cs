using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diwan.PL.Controllers
{
    public class NotificationController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<DiwanUser> _userManager;
        private readonly IMapper _mapper;

        public NotificationController(IUnitOfWork unitOfWork, UserManager<DiwanUser> userManager, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }
        public async Task<IActionResult> Notifications()
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
        public async Task<IActionResult> ReadNotification(int id,  string url)
        {
            var Notification = await _unitOfWork.NotificationRepository.FindFirstAsync(N => N.Id == id);
            if (Notification is null)
            {
                return NotFound();  
            }
            Notification.IsRead = true;
            _unitOfWork.NotificationRepository.Update(Notification);
            await _unitOfWork.CompleteAsync();
            return Redirect(url);
        }
    }
}
