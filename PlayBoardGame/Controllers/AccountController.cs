using System.Linq;
using System.Security.Claims;
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
        public async Task<IActionResult> Register(string returnUrl)
        {
            var model = new RegisterViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                TimeZoneList = ToolsExtensions.GetTimeZones()
            };
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View(model);
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
                    var appLink = Url.Action(nameof(Login), "Account", null, HttpContext.Request.Scheme);
                    var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                        {
                            IsHTML = true,
                            ToEmail = user.Email,
                            Subject = Constants.SubjectRegistrationEmail
                        }, Constants.TitleRegistrationEmail, Constants.ContentRegistrationEmail,
                        Constants.ButtonVisitSide,
                        appLink);

                    if (response.Successful) return RedirectToAction(nameof(StartController.Index), "Start");
                    TempData["ErrorMessage"] = Constants.GeneralSendEmailErrorMessage;
                    foreach (var error in response.Errors)
                    {
                        _logger.LogError(error);
                    }

                    return RedirectToAction(nameof(StartController.Index), "Start");
                }

                // bozy: Please provide users with something less technical BUT dump the errors to your log
                //It is not technical, it is for example: User name 'alex@example.com' is already taken. It is from Identity.

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(RegisterViewModel.Email), error.Description);
                    _logger.LogError(error.Description);
                }
            }

            vm.TimeZoneList = ToolsExtensions.GetTimeZones();
            vm.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return View(nameof(Register), vm);
        }

        public async Task<IActionResult> Login(string returnUrl)
        {
            var model = new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
            };
            if (!User?.Identity?.IsAuthenticated ?? false)
            {
                return View(model);
            }

            return RedirectToAction(nameof(ShelfController.List), "Shelf");
        }

        [HttpPost]
        public IActionResult ExternalLogin(string provider, string returnUrl, string timeZone,
            AuthPageType pageType = AuthPageType.Login)
        {
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account",
                new {ReturnUrl = returnUrl, TimeZone = timeZone, PageType = pageType});
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        public async Task<IActionResult> ExternalLoginCallback(string timeZone, AuthPageType pageType = AuthPageType.Login,
            string returnUrl = null, string remoteError = null)
        {
            var view = new ViewResult();
            var field = string.Empty;
            switch (pageType)
            {
                case AuthPageType.Login:
                    returnUrl = returnUrl ?? Url.Content("~/");
                    view = View("Login", new LoginViewModel
                    {
                        ReturnUrl = returnUrl,
                        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList()
                    });
                    field = nameof(LoginViewModel.Email);
                    break;
                case AuthPageType.Register:
                    returnUrl = Url.Action(nameof(StartController.Index), "Start");
                    field = nameof(RegisterViewModel.Email);
                    view = View("Register", new RegisterViewModel()
                    {
                        ReturnUrl = returnUrl,
                        ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList(),
                        TimeZoneList = ToolsExtensions.GetTimeZones()
                    });
                    break;
            }

            if (remoteError != null)
            {
                ModelState.AddModelError(field, Constants.AuthProviderError);
                _logger.LogError($"{Constants.AuthProviderKnownError}: {remoteError}");
                return view;
            }

            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                ModelState.AddModelError(field, Constants.AuthProviderError);
                _logger.LogError(Constants.AuthProviderUnknownError);
                return view;
            }

            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, false, true);
            if (result.Succeeded)
            {
                return LocalRedirect(returnUrl);
            }

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                ModelState.AddModelError(field, Constants.AuthProviderError);
                _logger.LogError(Constants.AuthProviderUnknownEmailError);
                return view;
            }
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = info.Principal.FindFirstValue(ClaimTypes.Email),
                    Email = info.Principal.FindFirstValue(ClaimTypes.Email),
                    TimeZone = timeZone ?? ToolsExtensions.GetTimeZones().FirstOrDefault().Key
                };
                var resultCreate = await _userManager.CreateAsync(user);
                if (resultCreate.Succeeded)
                {
                    var appLink = Url.Action(nameof(Login), "Account", null, HttpContext.Request.Scheme);
                    var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                        {
                            IsHTML = true,
                            ToEmail = user.Email,
                            Subject = Constants.SubjectRegistrationEmail
                        }, Constants.TitleRegistrationEmail, Constants.ContentRegistrationEmail,
                        Constants.ButtonVisitSide,
                        appLink);

                    if (!response.Successful)
                    {
                        TempData["ErrorMessage"] = Constants.GeneralSendEmailErrorMessage;
                        foreach (var error in response.Errors)
                        {
                            _logger.LogError(error);
                        }
                    }
                }
            }

            await _userManager.AddLoginAsync(user, info);
            await _signInManager.SignInAsync(user, false);

            return LocalRedirect(returnUrl);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginAsync(LoginViewModel vm)
        {
            vm.ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (!ModelState.IsValid) return View(nameof(Login), vm);
            var returnUrl = vm.ReturnUrl ?? Url.Action(nameof(ShelfController.List), "Shelf");
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
                var result = await _signInManager.PasswordSignInAsync(
                    user, vm.Password, false, false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }

                // bozy: what if something went bad
                // added:
                _logger.LogError($"Sign-in was not successful for {vm.Email}");
            }

            ModelState.AddModelError(nameof(LoginViewModel.Email), Constants.UserOrPasswordErrorMessage);

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
            if (!ModelState.IsValid) return View(nameof(SendResetPasswordLink), vm);
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);
                var resetLink = Url.Action(nameof(ResetPassword), "Account", new {emailToken = token},
                    protocol: HttpContext.Request.Scheme);
                var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                    {
                        IsHTML = true,
                        ToEmail = user.Email,
                        Subject = Constants.SubjectResetPasswordEmail
                    }, Constants.TitleResetPasswordEmail, Constants.ContentResetPasswordEmail,
                    Constants.ButtonResetPassword, resetLink);
                if (response.Successful)
                {
                    TempData["SuccessMessage"] = Constants.SendResetLinkSuccessMessage;
                    return RedirectToAction(nameof(Login));
                }

                _logger.LogCritical(resetLink);
                ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                    Constants.GeneralSendEmailErrorMessage);
                foreach (var error in response.Errors)
                {
                    _logger.LogError(error);
                }
            }

            ModelState.AddModelError(nameof(SendResetPasswordLinkViewModel.Email),
                Constants.LackOfEmailMatchMessage);

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
            if (!ModelState.IsValid) return View(nameof(ResetPassword), vm);
            var user = await _userManager.FindByEmailAsync(vm.Email);
            if (user != null)
            {
                if (!await _userManager.VerifyUserTokenAsync(user,
                    _userManager.Options.Tokens.PasswordResetTokenProvider, nameof(ResetPassword), vm.EmailToken))
                {
                    ModelState.AddModelError(nameof(ResetPasswordViewModel.Email),
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

                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(nameof(ResetPasswordViewModel.NewPassword),
                            error.Description);
                        _logger.LogError(error.Description);
                    }
                }
            }
            else
            {
                ModelState.AddModelError(nameof(ResetPasswordViewModel.Email),
                    Constants.LackOfEmailMatchMessage);
            }

            return View(nameof(ResetPassword), vm);
        }
    }
}