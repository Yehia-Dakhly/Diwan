using Diwan.DAL.Models;
using Diwan.PL.Helpers;
using Diwan.PL.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace Diwan.PL.Controllers
{
    public class AccountController : Controller
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
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(User);
                    var ConformationLink = Url.Action(nameof(ConfirmEmail), "Account", new { userId = User.Id, token }, Request.Scheme);
                    var emailBody = $@"
                    <html lang='ar' dir='rtl'>
                    <body style='font-family:Segoe UI, Tahoma, sans-serif; color:#333; background-color:#f9f9f9; padding:30px;'>
                        <div style='max-width:600px; margin:auto; background-color:#fff; padding:25px; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
                            <h2 style='color:#5c3317; text-align:center;'>تأكيد البريد الإلكتروني</h2>
                            <p>مرحباً،</p>
                            <p>شكرًا لتسجيلك في <strong>Diwan</strong>! قبل أن تبدأ، نحتاج فقط لتأكيد عنوان بريدك الإلكتروني.</p>

                            <div style='text-align:center; margin:30px 0;'>
                                <a href='{ConformationLink}' 
                                   style='background-color:#5c3317; color:#fff; padding:12px 25px; border-radius:6px; text-decoration:none; font-weight:bold;'>
                                   تأكيد البريد الإلكتروني
                                </a>
                            </div>

                            <p>إذا لم تقم بإنشاء حساب، يمكنك تجاهل هذه الرسالة بأمان.</p>
                            <p style='margin-top:30px;'>مع تحيات فريق <strong>Diwan</strong> ❤️</p>
                        </div>
                    </body>
                    </html>
                    ";
                    var email = new Email()
                    {
                        To = model.Email,
                        Subject = "تأكيد البريد الإلكتروني - Diwan",
                        Body = emailBody
                    };
                    await EmailSettings.SendEmailAsync(email);
                    return View("EmailVerification");
                }
                else
                {
                    foreach (var Error in Result.Errors)
                        ModelState.AddModelError(string.Empty, Error.Description);
                }
            }
            return View(model);
        }
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
                return View("EmailConfirmed");

            return View("Error");
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
                    if (await _userManager.IsEmailConfirmedAsync(User) == false)
                    {
                        ModelState.AddModelError(string.Empty, "من فضلك فعّل بريدك الإلكتروني قبل تسجيل الدخول.");
                        return View(model);
                    }
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
        public IActionResult ForgotPassword()
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
                    // https://localhost:44315/Account/ResetPassword?email=email@gmail.com?Token=token
                    var ResetPasswordLink = Url.Action("ResetPassword", "Account", new { email = User.Email, token = Token }, Request.Scheme);
                    var emailBody = $@"
                    <html>
                    <body style='font-family:Segoe UI, Tahoma, sans-serif; color:#333; background-color:#f9f9f9; padding:30px;'>
                        <div style='max-width:600px; margin:auto; background-color:#fff; padding:25px; border-radius:8px; box-shadow:0 2px 8px rgba(0,0,0,0.1);'>
                            <h2 style='color:#5c3317; text-align:center;'>إعادة تعيين كلمة المرور</h2>
                            <p>مرحباً،</p>
                            <p>لقد تلقينا طلباً لإعادة تعيين كلمة المرور الخاصة بحسابك. إذا كنت أنت من قام بهذا الطلب، فيرجى الضغط على الزر أدناه لإعادة التعيين:</p>
            
                            <div style='text-align:center; margin:30px 0;'>
                                <a href='{ResetPasswordLink}' 
                                   style='background-color:#5c3317; color:#fff; padding:12px 25px; border-radius:6px; text-decoration:none; font-weight:bold;'>
                                   إعادة تعيين كلمة المرور
                                </a>
                            </div>

                            <p>إذا لم تكن أنت من طلب ذلك، يمكنك تجاهل هذه الرسالة بأمان.</p>
                            <p style='margin-top:30px;'>مع تحيات فريق <strong>Diwan</strong> ❤️</p>
                        </div>
                    </body>
                    </html>
                    ";

                    var email = new Email()
                    {
                        To = model.Email,
                        Subject = "إعادة تعيين كلمة المرور - Diwan",
                        Body = emailBody
                    };
                    await EmailSettings.SendEmailAsync(email);
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

