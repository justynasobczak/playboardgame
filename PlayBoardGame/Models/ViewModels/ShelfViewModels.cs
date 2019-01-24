using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{
    public class ShelfListViewModel
    {
        public IEnumerable<Game> Shelf { get; set; }
    }

    public class ShelfEditViewModel
    {
        public IEnumerable<Game> Shelf { get; set; }
        public IEnumerable<Game> AvailableGames { get; set; }
    }

    public class ShelfModificationViewModel
    {
        [Required]
        public int[] IdsToAdd { get; set; }
        public int[] IdsToDelete { get; set; }
    }
}
