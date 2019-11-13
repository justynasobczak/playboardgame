using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace PlayBoardGame.Models.ViewModels
{
    public class GamesListViewModel
    {
        public List<Game> Games { get; set; }
    }

    public class CreateEditGameViewModel
    {
        public int GameId { get; set; }

        [Required] public string Title { get; set; }
        
        public IFormFile Photo { get; set; }
        
        public string PhotoPath { get; set; }
    }
}