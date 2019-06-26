using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class HangingTreeController : Controller
    {
        private readonly IGameRepository _gameRepository;

        public HangingTreeController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public ViewResult HangingTree()
        {
        
            var game = _gameRepository.Games.Any() ? 
                ToolsExtensions.Random(_gameRepository.Games) : new Game {Title = "Empty base", GameId = 44};
            
            return View(game);
        }
    }
}
