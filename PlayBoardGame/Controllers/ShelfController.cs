using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
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
                foreach (int gameId in model.IdsToAdd ?? new int[] { })
                {
                    _shelfRepository.SaveShelf(gameId);
                }
                return RedirectToAction("List");
            }
            return View(model);

        }
    }

}
