using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{
    public class GamesListViewModel
    {
        public IEnumerable<Game> Games { get; set; }
    }

    public class CreateEditGameViewModel
    {
        public int GameID { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
