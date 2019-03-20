using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private IShelfRepository _shelfRepository;

        public ShelfController(IShelfRepository shelfRepository)
        {
            _shelfRepository = shelfRepository;
        }

        public ViewResult List() => View(new
            ShelfListViewModel
        { Shelf = _shelfRepository.Shelf });

        public ViewResult Edit()
        {
            var mv = new ShelfEditViewModel
            {
                Shelf = _shelfRepository.Shelf,
                AvailableGames = _shelfRepository.AvailableGames
            };
            return View(mv);
        }

        [HttpPost]
        public IActionResult Edit(ShelfModificationViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (int gameId in model.IdsToAdd)
                {
                    _shelfRepository.AddToShelf(gameId);
                }

                foreach (int gameId in model.IdsToDelete)
                {
                    _shelfRepository.RemoveFromShelf(gameId);
                }
            }
            if (ModelState.IsValid)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction(nameof(List));
            }
            else
            {
                return Edit();
            }
        }
    }
}
