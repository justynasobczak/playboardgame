using PlayBoardGame.Infrastructure;

namespace PlayBoardGame.Models.ViewModels
{
    public class ShelfListViewModel
    {
        public PaginatedList<Game> Shelf { get; set; }
    }

    public class ShelfEditViewModel
    {
        public PaginatedList<Game> AvailableGames { get; set; }
    }
}