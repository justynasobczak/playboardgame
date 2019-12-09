using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using PlayBoardGame.Infrastructure;
using Microsoft.Extensions.Logging;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<UserController> _logger;

        public UserController(UserManager<AppUser> userManager, ILogger<UserController> logger)
        {
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<IActionResult> UserProfileAsync()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                _logger.LogCritical(Constants.UnknownId + " of user");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }

            var vm = new UserProfileViewModel
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                TimeZoneList = ToolsExtensions.GetTimeZones(),
                TimeZone = user.TimeZone,
                Address = new AddressViewModels
                {
                    Street = user.Street,
                    City = user.City,
                    Country = user.Country,
                    PostalCode = user.PostalCode
                }
            };
            return View("UserProfile", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UserProfileAsync(UserProfileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = vm.FirstName;
                    user.LastName = vm.LastName;
                    user.Street = vm.Address.Street;
                    user.City = vm.Address.City;
                    user.Country = vm.Address.Country;
                    user.PostalCode = vm.Address.PostalCode;
                    user.PhoneNumber = vm.PhoneNumber;
                    user.TimeZone = vm.TimeZone;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                        return RedirectToAction(nameof(UserProfileAsync));
                    }
                }

                _logger.LogCritical(Constants.UnknownId + " of user");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }

            vm.TimeZoneList = ToolsExtensions.GetTimeZones();
            return View("UserProfile", vm);
        }

        public IActionResult ChangePassword()
        {
            var vm = new ChangePasswordViewModel();

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel vm)
        {
            if (!ModelState.IsValid) return View(nameof(ChangePassword), vm);
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                _logger.LogCritical(Constants.UnknownId + " of user");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }

            if (!_userManager.HasPasswordAsync(user).Result)
            {
                ModelState.AddModelError(nameof(ChangePasswordViewModel.NewPassword), Constants.LoginByExternalProviderMessage);
                return View(nameof(ChangePassword), vm);
            }

            var isCorrectOld = await _userManager.CheckPasswordAsync(user, vm.OldPassword);
            if (isCorrectOld)
            {
                var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                    return View(nameof(ChangePassword));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(nameof(ChangePasswordViewModel.NewPassword), error.Description);
                    _logger.LogError(error.Description);
                    return View(nameof(ChangePassword), vm);
                }
            }

            ModelState.AddModelError(nameof(ChangePasswordViewModel.OldPassword),
                Constants.WrongOldPasswordMessage);
            return View(nameof(ChangePassword), vm);
        }
    }
}