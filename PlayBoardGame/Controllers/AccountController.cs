using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IEmailTemplateSender _templateSender;
        private readonly ILogger<AccountController> _logger;

        public AccountController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            IEmailTemplateSender templateSender, ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _templateSender = templateSender;
            _logger = logger;
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
                return View(new RegisterViewModel {TimeZoneList = ToolsExtensions.GetTimeZones()});
            }

            return RedirectToAction(nameof(ShelfController.List), "Shelf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAsync(RegisterViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = vm.Email,
                    Email = vm.Email,
                    TimeZone = vm.TimeZone
                };
                var result = await _userManager.CreateAsync(user, vm.Password);

                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, false);
                    var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                    {
                        IsHTML = true,
                        //ToEmail = user.Email,
                        ToEmail = "justyn.szczepan@gmail.com",
                        Subject = "Welcome to Let's play board game"
                    }, "Welcome to Let's play board game", "This is content", "This is button", "www.test.pl");

                    if (!response.Successful)
                    {
                        TempData["ErrorMessage"] = Constants.GeneralSendEmailErrorMessage;
                        foreach (var error in response.Errors)
                        {
                            _logger.LogError(error);
                        }
                    }

                    return RedirectToAction(nameof(StartController.Index), "Start");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(RegisterViewModel.Email), error.Description);
                    }
                }
            }
            
            vm.TimeZoneList = ToolsExtensions.GetTimeZones();
            return View(nameof(Register), vm);
        }

        public IActionResult Login()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }

            return RedirectToAction(nameof(ShelfController.List), "Shelf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(
                        user, vm.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToAction(nameof(ShelfController.List), "Shelf");
                    }
                }

                ModelState.AddModelError(nameof(LoginViewModel.Email), Constants.UserOrPasswordErrorMessage);
            }

            return View(nameof(Login), vm);
        }

        public async Task<IActionResult> LogoutAsync()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult SendResetPasswordLink()
        {
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View();
            }

            return RedirectToAction(nameof(ShelfController.List), "Shelf");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendResetPasswordLinkAsync(SendResetPasswordLinkViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var resetLink = Url.Action(nameof(ResetPassword), "Account", new {emailToken = token},
                        protocol: HttpContext.Request.Scheme);
                    var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                    {
                        IsHTML = true,
                        //ToEmail = user.Email,
                        ToEmail = "justyn.szczepan@gmail.com",
                        Subject = "Reset password"
                    }, "Reset password", "This is content", "This is button", resetLink);
                    if (response.Successful)
                    {
                        TempData["SuccessMessage"] = Constants.SendResetLinkSuccessMessage;
                        return RedirectToAction(nameof(Login));
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                            Constants.GeneralSendEmailErrorMessage);
                        foreach (var error in response.Errors)
                        {
                            _logger.LogError(error);
                        }
                    }
                }

                ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                    Constants.LackOfEmailMatchMessage);
            }

            return View(nameof(SendResetPasswordLink), vm);
        }

        public IActionResult ResetPassword(string emailToken)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPasswordAsync(ResetPasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(vm.Email);
                if (user != null)
                {
                    if (!await _userManager.VerifyUserTokenAsync(user,
                        _userManager.Options.Tokens.PasswordResetTokenProvider, nameof(ResetPassword), vm.EmailToken))
                    {
                        ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                            Constants.NotValidTokenMessage);
                    }
                    else
                    {
                        var result = await _userManager.ResetPasswordAsync(user, vm.EmailToken, vm.NewPassword);
                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                            return RedirectToAction(nameof(Login));
                        }

                        ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                            Constants.GeneralResetPasswordErrorMessage);
                    }
                }
                else
                {
                    ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                        Constants.LackOfEmailMatchMessage);
                }
            }

            return View(nameof(ResetPassword), vm);
        }
    }
}