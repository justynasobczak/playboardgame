using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using System.Linq;

namespace PlayBoardGame.Controllers
{
    public class GameController : Controller
    {
        private IGameRepository _gameRepository;

        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public ViewResult List() => View(new GamesListViewModel { Games = _gameRepository.Games });

        public IActionResult Edit(int GameID)
        {
            Game game = _gameRepository.Games.FirstOrDefault(g => g.GameID == GameID);
            if (game != null)
            {
                var vm = new CreateEditGameViewModel
                {
                    Title = game.Title,
                    GameID = game.GameID
                };
                return View(vm);
            };
            return RedirectToAction("Error", "Error");
        }

        [HttpPost]
        public IActionResult Edit(CreateEditGameViewModel game)
        {
            if (ModelState.IsValid)
            {
                _gameRepository.SaveGame(new Game { Title = game.Title, GameID = game.GameID });
                return RedirectToAction("List");
            }
            else
            {
                return View(game);
            }
        }

        public ViewResult Create() => View("Edit", new CreateEditGameViewModel());

        [HttpPost]
        public IActionResult Delete(int gameID)
        {
            Game deletedGame = _gameRepository.DeleteGame(gameID);
            return RedirectToAction("List");
        }
    }
}
