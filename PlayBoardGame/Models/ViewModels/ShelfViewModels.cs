using System.Collections.Generic;

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
        public IEnumerable<int> IdsToAdd { get; } = new List<int>();
        public IEnumerable<int> IdsToDelete { get; } = new List<int>();
    }
}