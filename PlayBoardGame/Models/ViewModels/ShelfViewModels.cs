using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models.ViewModels
{
    public class ShelfListViewModel
    {
        public IQueryable<Game> Shelf { get; set; }
    }

    public class ShelfEditViewModel
    {
        public IQueryable<Game> Shelf { get; set; }
        public IQueryable<Game> AvailableGames { get; set; }
    }

    public class ShelfModificationViewModel
    {
        public IEnumerable<int> IdsToAdd { get; } = new List<int>();
        public IEnumerable<int> IdsToDelete { get; } = new List<int>();
    }
}