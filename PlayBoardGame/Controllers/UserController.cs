using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private UserManager<AppUser> _userManager;
        private SignInManager<AppUser> _signInManager;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> UserProfile()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var vm = new UserProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Street = user.Street,
                    City = user.City,
                    PostalCode = user.PostalCode,
                    PhoneNumber = user.PhoneNumber
                };
                return View(vm);
            };
            return RedirectToAction("Error", "Error");
        }

        [HttpPost]
        public async Task<IActionResult> UserProfile(UserProfileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = vm.FirstName;
                    user.LastName = vm.LastName;
                    user.Street = vm.Street;
                    user.City = vm.City;
                    user.PostalCode = vm.PostalCode;
                    user.PhoneNumber = vm.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        return RedirectToAction("List", "Shelf");
                    }
                    return RedirectToAction("Error", "Error");
                }
                return RedirectToAction("Error", "Error");
            }
            else
            {
                return View(vm);
            }
        }
    }
}
