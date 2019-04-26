using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using System.Linq;

namespace PlayBoardGame.Controllers
{
    [Authorize(Roles = "Admins")]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;

        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public ViewResult List() => View(new GamesListViewModel {Games = _gameRepository.Games});

        public IActionResult Edit(int id)
        {
            var game = _gameRepository.Games.FirstOrDefault(g => g.GameID == id);
            if (game != null)
            {
                var vm = new CreateEditGameViewModel
                {
                    Title = game.Title,
                    GameID = game.GameID
                };
                return View(vm);
            }
            return RedirectToAction("Error", "Error");
        }

        [HttpPost]
        public IActionResult Edit(CreateEditGameViewModel game)
        {
            if (ModelState.IsValid)
            {
                _gameRepository.SaveGame(new Game {Title = game.Title, GameID = game.GameID});
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction("List");
            }
            return View(game);
        }

        public ViewResult Create() => View("Edit", new CreateEditGameViewModel());

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var deletedGame = _gameRepository.DeleteGame(id);
            if (deletedGame != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }
            return RedirectToAction("List");
        }
    }
}