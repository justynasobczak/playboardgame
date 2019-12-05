using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Infrastructure;
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

        public async Task<ViewResult> List(string currentFilter, string searchString,
            int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var currentUserId = GetCurrentUserId().Result;
            var shelf = _shelfRepository.GetShelfForUser(currentUserId).OrderBy(g => g.Title);
            if (!string.IsNullOrEmpty(searchString))
            {
                shelf = shelf.Where(g => g.Title.Contains(searchString)).OrderBy(g => g.Title);
            }

            var mv = new ShelfListViewModel
            {
                Shelf =
                    await PaginatedList<Game>.CreateAsync(shelf, pageNumber ?? 1, Constants.PageSize)
            };
            return View(mv);
        }

        public async Task<ViewResult> Edit(string currentFilter, string searchString,
            int? pageNumber)
        {
            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;
            var currentUserId = GetCurrentUserId().Result;
            var availableGames = _shelfRepository.GetAvailableGamesForUser(currentUserId).OrderBy(g => g.Title);
            if (!string.IsNullOrEmpty(searchString))
            {
                availableGames = availableGames.Where(g => g.Title.Contains(searchString)).OrderBy(g => g.Title);
            }

            var mv = new ShelfEditViewModel
            {
                AvailableGames =
                    await PaginatedList<Game>.CreateAsync(availableGames, pageNumber ?? 1, Constants.PicturesNumber)
            };
            return View(mv);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id)
        {
            var currentUserId = GetCurrentUserId().Result;

            var gameAppUser = new GameAppUser {UserId = currentUserId, GameId = id};
            _shelfRepository.AddToShelf(gameAppUser);

            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(Edit));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var currentUserId = GetCurrentUserId().Result;

            var gameAppUser = new GameAppUser {UserId = currentUserId, GameId = id};
            var deletedGameAppUser = _shelfRepository.RemoveFromShelf(gameAppUser);

            if (deletedGameAppUser != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction(nameof(List));
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}