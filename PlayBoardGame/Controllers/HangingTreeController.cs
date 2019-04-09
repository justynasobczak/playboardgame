using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    public class HangingTreeController : Controller
    {
        private IGameRepository _gameRepository;

        public HangingTreeController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public ViewResult HangingTree()
        {
            Game game = ToolsExtensions.Random(_gameRepository.Games);
            return View(game);
        }
    }
}
