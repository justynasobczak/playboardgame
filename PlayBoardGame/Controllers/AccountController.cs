using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;
        private IEmailTemplateSender _templateSender;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IEmailTemplateSender templateSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _templateSender = templateSender;
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }
            return RedirectToAction("List", "Shelf");
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = new AppUser
                {
                    UserName = vm.Email,
                    Email = vm.Email
                };
                IdentityResult result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    SendEmailResponse response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                    {
                        IsHTML = true,
                        //ToEmail = user.Email,
                        ToEmail = "justyn.szczepan@gmail.com",
                        Subject = "Welcome to Let's play board game"
                    }, "Welcome to Let's play board game", "This is content", "This is button", "www.test.pl");

                    if (!response.Successful)
                    {
                        TempData["EmailErrorMessage"] = "Please contact support because of unknown error from email sending server.";
                    }

                    return RedirectToAction("List", "Shelf");
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(RegisterViewModel.Email), error.Description);
                    }
                }
            }
            return View(vm);
        }

        public IActionResult Login()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }
            return RedirectToAction("List", "Shelf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(
                        user, vm.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("List", "Shelf");
                    }
                }
                ModelState.AddModelError(nameof(LoginViewModel.Email), "Invalid user or password");
            }
            return View(vm);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult SendResetPasswordLink()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }
            return RedirectToAction("List", "Shelf");
        }

        [HttpPost]
        public async Task<IActionResult> SendResetPasswordLink(SendResetPasswordLinkViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action("ResetPassword", "Account", new { emailToken = token },
                        protocol: HttpContext.Request.Scheme);
                    SendEmailResponse response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                    {
                        IsHTML = true,
                        //ToEmail = user.Email,
                        ToEmail = "justyn.szczepan@gmail.com",
                        Subject = "Reset password"
                    }, "Reset password", "This is content", "This is button", resetLink);
                    if (response.Successful)
                    {
                        //TO DO: confirmation message
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                            "Please contact support because of unknown error from email sending server.");
                    }
                }
                ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                    "Entered email address doesn't match any account");
            }
            return View(vm);
        }

        public IActionResult ResetPassword(string emailToken)
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    var result = await _userManager.ResetPasswordAsync(user, vm.EmailToken, vm.NewPassword);
                    if (result.Succeeded)
                    {
                        //TO DO: confirmation message
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email), "Error while resetting the password");
                    }
                } else
                {
                    ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                    "Entered email address doesn't match any account");
                }
            }
            return View(vm);
        }
    }
}