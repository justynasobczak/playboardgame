using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private readonly IShelfRepository _shelfRepository;
        private readonly UserManager<AppUser> _userManager;

        public ShelfController(IShelfRepository shelfRepository, UserManager<AppUser> userManager)
        {
            _shelfRepository = shelfRepository;
            _userManager = userManager;
        }

        public ViewResult List()
        {
            var currentUserId = GetCurrentUserId().Result;
            return View(new ShelfListViewModel {Shelf = _shelfRepository.GetShelfForUser(currentUserId)});
        }

        public ViewResult Edit()
        {
            var currentUserId = GetCurrentUserId().Result;
            var mv = new ShelfEditViewModel
            {
                Shelf = _shelfRepository.GetShelfForUser(currentUserId),
                AvailableGames = _shelfRepository.GetAvailableGamesForUser(currentUserId)
            };
            return View(mv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(ShelfModificationViewModel model)
        {
            var currentUserId = GetCurrentUserId().Result;
            if (!ModelState.IsValid) return Edit();
            foreach (var gameId in model.IdsToAdd)
            {
                var gameAppUser = new GameAppUser {UserId = currentUserId, GameId = gameId};
                _shelfRepository.AddToShelf(gameAppUser);
            }

            foreach (var gameId in model.IdsToDelete)
            {
                var gameAppUser = new GameAppUser {UserId = currentUserId, GameId = gameId};
                _shelfRepository.RemoveFromShelf(gameAppUser);
            }

            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(List));
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}