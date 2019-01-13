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

        public ViewResult List() => View(new GamesListViewModel { Games = _gameRepository.Games});

        public ViewResult Edit(int GameID) => View(_gameRepository.Games.FirstOrDefault(g => g.GameID == GameID) );

        [HttpPost]
        public IActionResult Edit(Game game)
        {
            if (ModelState.IsValid)
            {
                _gameRepository.SaveGame(game);
                return RedirectToAction("List");
            } else
            {
                return View(game);
            }
        }
        
        public ViewResult Create() => View("Edit", new Game());

        [HttpPost]
        public IActionResult Delete(int gameID)
        {
            Game deletedGame = _gameRepository.DeleteGame(gameID);
            return RedirectToAction("List");
        }
    }
}
