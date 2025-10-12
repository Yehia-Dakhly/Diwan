
using Demo.PL.Helpers;
using Diwan.DAL.Models;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Diwan.PL.Controllers
{
    public class AccountController: Controller
    {
        private readonly UserManager<DiwanUser> _userManager;
        private readonly SignInManager<DiwanUser> _signInManager;

        public AccountController(UserManager<DiwanUser> userManager, SignInManager<DiwanUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }
        #region Register
        // BaseURL/Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var User = new DiwanUser()
                {
                    UserName = model.Email.Split("@")[0],
                    Email = model.Email,
                    FirstName = model.FName,
                    LastName = model.LName,
                    Bio = model.Bio,
                    CoverPicURL = model.CoverURL,
                    DateOfBirth = model.DateOfBirth,
                    Gender = model.Gender,
                    PictureURL = model.PictureURL,
                    IsAgree = model.IsAgree,
                    PhoneNumber = model.PhoneNumber,
                };
                if (model.PictureImage is not null)
                    User.PictureURL = Helpers.DocumentSettings.UploadFile(model.PictureImage, "Images");
                if (model.CoverImage is not null)
                    User.CoverPicURL = Helpers.DocumentSettings.UploadFile(model.CoverImage, "Images");
                var Result = await _userManager.CreateAsync(User, model.Password);
                if (Result.Succeeded)
                {
                    
                    return RedirectToAction(nameof(Login));
                }
                else
                {
                    foreach (var Error in Result.Errors)
                        ModelState.AddModelError(string.Empty, Error.Description);
                }
            }
            return View(model);
        }
        #endregion

        #region Sign In - Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid) // Server Side Validation
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if (User is not null)
                {
                    // Login
                    var Result = await _userManager.CheckPasswordAsync(User, model.Password);
                    if (Result)
                    {
                        // Login
                        var LoginResult = await _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false); // Create Token
                        if (LoginResult.Succeeded)
                            return RedirectToAction("Index", "Home");

                    }
                    else
                        ModelState.AddModelError(string.Empty, "Password is Incorrect!");

                }
                else
                    ModelState.AddModelError(string.Empty, "Email is not Exists");
            }
            return View(model);
        }
        #endregion

        #region Sign Out
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }
        #endregion

        #region Forget Password
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmail(ForgetPassswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var User = await _userManager.FindByEmailAsync(model.Email);
                if (User is not null)
                {
                    var Token = await _userManager.GeneratePasswordResetTokenAsync(User);
                    // Valid For Only One Time For This User!
                    // https://localhost:44315/Account/ResetPassword?email=yehiadakhly2004@gmail.com?Token=ajkbgfnkkldengnlkndnklkgfdsn
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = User.Email, token = Token }, Request.Scheme);
                    var Email = new Email()
                    {
                        To = model.Email,
                        Subject = "Reset Password",
                        Body = ResetPasswordLink
                    };
                    EmailSettings.SendEmail(Email);
                    return RedirectToAction(nameof(CheckYourIndox));
                }
                else
                    ModelState.AddModelError(string.Empty, "Email is not Exists!");
            }
            return View("ForgetPassword", model);
            // Reset Password
        }
        public IActionResult CheckYourIndox()
        {
            return View();
        }
        #endregion

        #region Reset Password
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                string email = TempData["email"] as string;
                string token = TempData["token"] as string;
                var User = await _userManager.FindByEmailAsync(email);
                var Result = await _userManager.ResetPasswordAsync(User, token, model.NewPassword);
                if (Result.Succeeded)
                    return RedirectToAction(nameof(Login));
                else
                    foreach (var Error in Result.Errors)
                        ModelState.AddModelError(string.Empty, Error.Description);
            }
            return View(model);
        }
        #endregion
    }
}

