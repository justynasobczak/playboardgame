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
                        FromEmail = "playboardgame@test.com",
                        FromName = "Let's play boardgame",
                        //ToEmail = user.Email,
                        ToEmail = "justyn.szczepan@gmail.com",
                        Subject = "Welcome to Let's play boardgame"
                    }, "Welcome to Let's play boardgame", "This is content", "This is button", "www.test.pl");

                    if (!response.Successful)
                    {
                        TempData["EmailErrorMessage"] = "Please contact support because of unknow error from email sending server.";
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
    }
}