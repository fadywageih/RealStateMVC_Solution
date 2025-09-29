using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealState.BLL.Common.Services.EmailSettings;
using RealState.BLL.Models.User;
using RealState.DAL.Models.Identity;
using RealState.DAL.Models.Users;
using RealState.DAL.Presistance.Data;
using RealState.Models.Account;

namespace RealState.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSettings _emailSettings;
        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager
    , IEmailSettings emailSettings)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSettings = emailSettings;
        }
        #region Register
        #region Get
        [HttpGet]
        public async Task<IActionResult> SignUp()
        {
            return View();
        }
        #endregion
        #region Post
        [HttpPost]
        public async Task<IActionResult> SignUp(SignUpViewModel signUpViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(signUpViewModel); 
            }
            //Check if user exist already exists
            var existingUser = await _userManager.FindByNameAsync(signUpViewModel.UserName);
            if (existingUser != null)
            {
                ModelState.AddModelError(nameof(SignUpViewModel.UserName), "User already exists");
                return View(signUpViewModel);
            }

            var user = new ApplicationUser()
            {
                FName = signUpViewModel.FirstName,
                LName = signUpViewModel.LastName,
                UserName = signUpViewModel.UserName,
                Email = signUpViewModel.Email,
                ISAgress = signUpViewModel.IsAgree
            };
            var result = await _userManager.CreateAsync(user, signUpViewModel.Password);
            if (result.Succeeded)
            {
                var customUser = new User
                {
                    Name = $"{signUpViewModel.FirstName} {signUpViewModel.LastName}",
                    Email = signUpViewModel.Email,
                    IdentityUserId = user.Id
                };

                // استخدم UnitOfWork أو DbContext مباشرةً
                using var scope = HttpContext.RequestServices.CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
                dbContext.Users.Add(customUser);
                await dbContext.SaveChangesAsync();
                return RedirectToAction(nameof(SignIn));
            }
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View(signUpViewModel);
        }
        #endregion
        #endregion
        #region Login
        #region Get
        [HttpGet]
        public async Task<IActionResult> SignIn()
        {
            return View();
        }
        #endregion
        #region Post
        [HttpPost]
        public async Task<IActionResult> SignIn(SignInViewModel signInViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByEmailAsync(signInViewModel.Email);
            if (user is { })
            {
                var flag = await _userManager.CheckPasswordAsync(user, signInViewModel.Password);
                if (flag)
                {
                    var result1 = await _signInManager.PasswordSignInAsync(user, signInViewModel.Password,
                        signInViewModel.RememberMe, true);
                    if (result1.IsNotAllowed)
                    {
                        ModelState.AddModelError(string.Empty, "You are not allowed to login. Please contact administrator.");
                    }
                    if (result1.IsLockedOut)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is locked. Please try again later.");
                    }
                    if (result1.Succeeded)
                    {
                        return RedirectToAction(nameof(HomeController.Index), "Home");
                    }
                }
            }
            ModelState.AddModelError(string.Empty, "Invalid Email or Password");
            return View(signInViewModel);
        }
        #endregion
        #endregion
        #region LogOut
        public async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(SignIn));
        }
        #endregion
        #region Forget Password
        #region Get
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            return View();
        }
        #endregion
        #region post
        [HttpPost]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user is not null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var url = Url.Action("ResetPassword", "Account",
                        new { email = model.Email, token = token }, Request.Scheme);
                    //To,subject,Body
                    var email = new Email()
                    {
                        To = user.Email,
                        Subject = "Reset your Password",
                        Body = url
                    };
                    //send email
                    _emailSettings.SendEmail(email);
                    return RedirectToAction("CheckYourInbox");

                }
                ModelState.AddModelError(string.Empty, "Invalid Email");
            }
            return View("ForgetPassword", model);
        }

        #endregion
        #endregion
        #region Check Your inbox
        [HttpGet]
        public IActionResult CheckYourInbox()
        {
            return View();
        }
        #endregion
        #region Reset Password
        //new Password,ConfirmPassword
        //ResetPassword
        [HttpGet]
        public IActionResult ResetPassword(string email, string token)
        {
            TempData["email"] = email;
            TempData["token"] = token;
            //Pass email and token
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var email = TempData["email"] as string;
                var token = TempData["token"] as string;
                var user = await _userManager.FindByEmailAsync(email);
                if (user is not null)
                {
                    var result1 = await _userManager.ResetPasswordAsync(user, token, model.Password);
                    if (result1.Succeeded)
                    {
                        return RedirectToAction(nameof(SignIn));
                    }
                }

            }
            ModelState.AddModelError(string.Empty, "Something went wrong");
            return View(model);
        }
        #endregion
        #region Setting
        // في AccountController.cs

        [Authorize]
        public async Task<IActionResult> Settings()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            var model = new UserSettingsViewModel
            {
                FirstName = user.FName,
                LastName = user.LName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber
            };

            return View(model);
        }

        [HttpPost, Authorize]
        public async Task<IActionResult> Settings(UserSettingsViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();

            // تحديث البيانات الأساسية
            user.UserName = model.UserName; // <-- أضف هذا السطر

            user.FName = model.FirstName;
            user.LName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            // لو المستخدم عاوز يغيّر الإيميل
            if (model.Email != user.Email)
            {
                var setEmailResult = await _userManager.SetEmailAsync(user, model.Email);
                if (!setEmailResult.Succeeded)
                {
                    foreach (var error in setEmailResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
                user.UserName = model.Email; // لو الـ UserName = Email
            }

            // لو المستخدم عاوز يغيّر الباسوورد
            if (!string.IsNullOrEmpty(model.NewPassword))
            {
                if (string.IsNullOrEmpty(model.CurrentPassword))
                {
                    ModelState.AddModelError(string.Empty, "Current password is required to change password.");
                    return View(model);
                }

                var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
                if (!passwordCheck)
                {
                    ModelState.AddModelError(string.Empty, "Current password is incorrect.");
                    return View(model);
                }

                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var changePassResult = await _userManager.ResetPasswordAsync(user, token, model.NewPassword);
                if (!changePassResult.Succeeded)
                {
                    foreach (var error in changePassResult.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                    return View(model);
                }
            }

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user); 
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
                return View(model);
            }

            TempData["SuccessMessage"] = "Your profile has been updated successfully.";
            return RedirectToAction(nameof(Settings));
        }
        #endregion
    }
}
