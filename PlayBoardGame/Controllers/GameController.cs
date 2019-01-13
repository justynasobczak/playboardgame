using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    public class GameController : Controller
    {
        private IGameRepository _gameRepository;

        public GameController(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public ViewResult List() => View(_gameRepository.Games);
    }
}
