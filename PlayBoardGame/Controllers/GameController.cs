using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;

namespace PlayBoardGame.Controllers
{
    [Authorize(Roles = "Admins")]
    public class GameController : Controller
    {
        private readonly IGameRepository _gameRepository;
        private readonly ILogger<GameController> _logger;
        private readonly IHostingEnvironment _hostingEnvironment;

        public GameController(IGameRepository gameRepository, ILogger<GameController> logger, 
            IHostingEnvironment hostingEnvironment)
        {
            _gameRepository = gameRepository;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        //TODO paging
        public ViewResult List() => View(new GamesListViewModel {Games = _gameRepository.Games.ToList()});

        public IActionResult Edit(int id)
        {
            // bozy: linq: var game1 = (from item in _gameRepository.Games where item.GameId == id select item).FirstOrDefault();
            var game = _gameRepository.GetGame(id);
            if (game != null)
            {
                var vm = new CreateEditGameViewModel
                {
                    Title = game.Title,
                    GameId = game.GameId
                };
                return View(vm);
            }

            _logger.LogCritical(Constants.UnknownId + " of game");
            return RedirectToAction(nameof(ErrorController.Error), "Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CreateEditGameViewModel game)
        {
            if (!ModelState.IsValid) return View(game);
            string uniqueFileName = null;
            if (game.Photo != null)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "gamePhotos");
                uniqueFileName = $"{Guid.NewGuid().ToString()}_{game.Photo.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                game.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
            }
            _gameRepository.SaveGame(new Game {Title = game.Title, GameId = game.GameId, PhotoPath =  uniqueFileName});
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(List));
        }

        public ViewResult Create() => View(nameof(Edit), new CreateEditGameViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            var deletedGame = _gameRepository.DeleteGame(id);
            if (deletedGame != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction(nameof(List));
        }
    }
}