using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PlayBoardGame.Models;

namespace PlayBoardGame.Models.ViewModels
{
    public class GamesListViewModel
    {
        public IEnumerable<Game> Games { get; set; }
    }
}
