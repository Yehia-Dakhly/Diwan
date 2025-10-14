using AutoMapper;
using Diwan.BLL.Interfaces;
using Diwan.BLL.Repositories;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;
using System.Threading.Tasks;

namespace Diwan.PL.Controllers
{
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
            var CurrentUser = _userManager.GetUserId(User);
            if (CurrentUser is null)
            {
                return RedirectToAction("Login", "Account");
            }
            var Comment = new Comment()
            {
                Content = model.Content,
                UserId = CurrentUser,
                PostId = model.PostId,
            };
            await _unitOfWork.CommentRepository.AddAsync(Comment);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction("Index", "Home");
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

                // 3. تجميع الردود تحت التعليقات الأصلية (الأهم)
                var commentLookup = allCommentViewModels.ToDictionary(c => c.Id);
                var topLevelComments = new List<CommentViewModel>();

                foreach (var comment in allCommentViewModels)
                {
                    // إذا كان للتعليق أب، أضفه إلى قائمة الردود الخاصة بالأب
                    if (comment.ParentId != null && commentLookup.ContainsKey(comment.ParentId.Value))
                    {
                        commentLookup[comment.ParentId.Value].Replies.Add(comment);
                    }
                    // إذا لم يكن له أب، فهو تعليق أساسي
                    else
                    {
                        topLevelComments.Add(comment);
                    }
                }

                // رتب التعليقات والردود حسب التاريخ
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
                var reply = new Comment
                {
                    Content = model.Content,
                    PostId = model.PostId,
                    ParentId = model.ParentId,
                    UserId = _userManager.GetUserId(User)
                };

                await _unitOfWork.CommentRepository.AddAsync(reply);
                await _unitOfWork.CompleteAsync();

                return RedirectToAction("Index", "Home");
            }
            return BadRequest("Invalid reply data.");
        }
        public async Task<IActionResult> Delete(int id)
        {
            int Temp = id;
            List<Comment> V = new List<Comment>();
            V.Add(await _unitOfWork.CommentRepository.FindFirstAsync(F => F.Id == id));
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
            return RedirectToAction("Index", "Home");
        }
    }
}
