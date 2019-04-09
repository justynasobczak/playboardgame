﻿using System;
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

        public async Task<IActionResult> UserProfileAsync()
        {
            AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user != null)
            {
                var vm = new UserProfileViewModel
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    PhoneNumber = user.PhoneNumber,
                    Address = new AddressViewModels
                    {
                        Street = user.Street,
                        City = user.City,
                        PostalCode = user.PostalCode,
                    }
                };
                if (user.Country != null)
                {
                    if (Enum.TryParse(user.Country, out CountryEnum myCountry))
                    {
                        vm.Address.EnumCountry = myCountry;
                    }
                    //TODO: Add log if failed
                } else
                {
                    vm.Address.EnumCountry = CountryEnum.None;
                }
                return View("UserProfile", vm);
            };
            return RedirectToAction("Error", "Error");
        }

        [HttpPost]
        public async Task<IActionResult> UserProfileAsync(UserProfileViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    user.FirstName = vm.FirstName;
                    user.LastName = vm.LastName;
                    user.Street = vm.Address.Street;
                    user.City = vm.Address.City;
                    user.Country = vm.Address.EnumCountry.ToString();
                    user.PostalCode = vm.Address.PostalCode;
                    user.PhoneNumber = vm.PhoneNumber;

                    var result = await _userManager.UpdateAsync(user);
                    if (result.Succeeded)
                    {
                        TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                        return RedirectToAction("List", "Shelf");
                    }
                }
                return RedirectToAction("Error", "Error");
            }
            else
            {
                return View("UserProfile", vm);
            }
        }

        public IActionResult ChangePassword()
        {
            var vm = new ChangePasswordViewModel();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePasswordAsync(ChangePasswordViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user != null)
                {
                    var isCorrectOld = await _userManager.CheckPasswordAsync(user, vm.OldPassword);
                    if (isCorrectOld)
                    {
                        var result = await _userManager.ChangePasswordAsync(user, vm.OldPassword, vm.NewPassword);
                        if (result.Succeeded)
                        {
                            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                            return RedirectToAction("List", "Shelf");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError(nameof(ChangePasswordViewModel.OldPassword), Constants.WrongOldPasswordMessage);
                        return View("ChangePassword", vm);
                    }
                }
                return RedirectToAction("Error", "Error");
            }
            else
            {
                return View("ChangePassword", vm);
            }
        }
    }
}