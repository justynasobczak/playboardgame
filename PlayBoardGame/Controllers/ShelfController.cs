using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class ShelfController : Controller
    {
        private readonly IShelfRepository _shelfRepository;

        public ShelfController(IShelfRepository shelfRepository)
        {   
            _shelfRepository = shelfRepository;
        }

        public ViewResult List()
        {
            return View(new ShelfListViewModel {Shelf = _shelfRepository.Shelf});
        }

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
                foreach (var gameId in model.IdsToAdd)
                {
                    _shelfRepository.AddToShelf(gameId);
                }

                foreach (var gameId in model.IdsToDelete)
                {
                    _shelfRepository.RemoveFromShelf(gameId);
                }
                
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction(nameof(List));
                
            }
            return Edit();
        }
    }
}