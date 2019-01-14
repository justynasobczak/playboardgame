﻿using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using PlayBoardGame.Models;

namespace PlayBoardGame.Models.ViewModels
{
    public class GamesListViewModel
    {
        public IEnumerable<Game> Games { get; set; }
    }

    public class CreateEditGameViewModel
    {
        public int GameID { get; set; }
        [Required(ErrorMessage = "Please enter a game name")]
        public string Title { get; set; }
    }
}
