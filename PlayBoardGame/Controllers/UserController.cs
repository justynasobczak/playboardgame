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
            if (user != null)
            {
                var vm = new UserProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
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
            _logger.LogCritical(Constants.UnknownId + " of user");
            return RedirectToAction(nameof(ErrorController.Error), "Error");
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
                        return RedirectToAction(nameof(ShelfController.List), "Shelf");
                    }
                }
                _logger.LogCritical(Constants.UnknownId + " of user");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }
            else
            {
                vm.TimeZoneList = ToolsExtensions.GetTimeZones();
                return View("UserProfile", vm);
            }
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
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    var isCorrectOld = await _userManager.CheckPasswordAsync(user, vm.OldPassword);
                    if (isCorrectOld)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);
                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                            return RedirectToAction(nameof(ShelfController.List), "Shelf");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(ChangePasswordViewModel.OldPassword), Constants.WrongOldPasswordMessage);
                        return View("ChangePassword", vm);
                    }
                }
                _logger.LogCritical(Constants.UnknownId + " of user");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }
            else
            {
                return View("ChangePassword", vm);
            }
        }
    }
}
