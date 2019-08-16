﻿using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PlayBoardGame.Models.ViewModels
{
    public class GamesListViewModel
    {
        public IQueryable<Game> Games { get; set; }
    }

    public class CreateEditGameViewModel
    {
        public int GameId { get; set; }

        [Required]
        public string Title { get; set; }
    }
}
