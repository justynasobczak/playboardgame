using System;
using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Infrastructure;

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
        
        public async Task<IActionResult> List(string sortOrder, string currentFilter, string searchString,
            int? pageNumber)
        {
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParm"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

            if (searchString != null)
            {
                pageNumber = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewData["CurrentFilter"] = searchString;

            var games = _gameRepository.Games;
            if (!string.IsNullOrEmpty(searchString))
            {
                games = games.Where(g => g.Title.Contains(searchString));
            }

            switch (sortOrder)
            {
                case "title_desc":
                    games = games.OrderByDescending(g => g.Title);
                    break;
                default:
                    games = games.OrderBy(g => g.Title);
                    break;
            }
            
            return View(new GamesListViewModel
                {Games = await PaginatedList<Game>.CreateAsync(games, pageNumber ?? 1, Constants.PageSize)});
        }

        public IActionResult Edit(int id)
        {
            // bozy: linq: var game1 = (from item in _gameRepository.Games where item.GameId == id select item).FirstOrDefault();
            var game = _gameRepository.GetGame(id);
            if (game != null)
            {
                var vm = new CreateEditGameViewModel
                {
                    Title = game.Title,
                    GameId = game.GameId,
                    PhotoPath = game.PhotoPath
                };
                return View(vm);
            }

            _logger.LogCritical(Constants.UnknownId + " of game");
            return RedirectToAction(nameof(ErrorController.Error), "Error");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CreateEditGameViewModel game)
        {
            string[] permittedExtensions = {".jpg", ".png"};
            string extension = null;
            if (game.Photo != null)
            {
                extension = Path.GetExtension(game.Photo.FileName).ToLowerInvariant();
                if (string.IsNullOrEmpty(extension) || !permittedExtensions.Contains(extension))
                {
                    ModelState.AddModelError(nameof(CreateEditGameViewModel.Photo),
                        $"{Constants.WrongFileExtension}, allowed file types: {string.Join(" ,", permittedExtensions)}.");
                }

                if (game.Photo.Length > Constants.FileSizeLimit)
                {
                    ModelState.AddModelError(nameof(CreateEditGameViewModel.Photo),
                        $"{Constants.WrongFileSize}, max: {Constants.FileSizeLimit.ToString()}.");
                }
            }

            if (!ModelState.IsValid) return View(game);
            var editedGame = _gameRepository.GetGame(game.GameId) ?? new Game();
            var uniqueFileName = editedGame.PhotoPath;
            var fileName = editedGame.PhotoName;
            if (game.Photo != null)
            {
                var uploadsFolder = SetUploadsFolder();
                if (editedGame.PhotoPath != null)
                {
                    DeleteGamePhoto(editedGame, uploadsFolder);
                }

                uniqueFileName = $"{Guid.NewGuid().ToString()}{extension}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                fileName = game.Photo.FileName;
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await game.Photo.CopyToAsync(fileStream);
                }
            }

            _gameRepository.SaveGame(new Game
            {
                Title = game.Title, GameId = game.GameId,
                PhotoPath = uniqueFileName, PhotoName = fileName
            });
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(List));
        }

        public ViewResult Create() => View(nameof(Edit), new CreateEditGameViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            if (_gameRepository.IsGameInMeeting(id))
            {
                TempData["ErrorMessage"] = Constants.GameConnectedWithMeetingErrorMessage;
                return RedirectToAction(nameof(List));
            }
            var deletedGame = _gameRepository.DeleteGame(id);
            if (deletedGame != null && deletedGame.PhotoPath != null)
            {
                var uploadsFolder = SetUploadsFolder();
                DeleteGamePhoto(deletedGame, uploadsFolder);
            }

            if (deletedGame != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction(nameof(List));
        }

        private string SetUploadsFolder()
        {
            return Path.Combine(_hostingEnvironment.WebRootPath, "gamephotos");
        }

        private void DeleteGamePhoto(Game game, string uploadsFolder)
        {
            var fileToRemove = $"/{uploadsFolder}/{game.PhotoPath}";
            if (System.IO.File.Exists(fileToRemove))
            {
                System.IO.File.Delete(fileToRemove);
            }
            else
            {
                _logger.LogError($"{Constants.CannotRemoveFile}: {game.PhotoName}");
            }
        }
    }
}