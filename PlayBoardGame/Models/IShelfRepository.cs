using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IShelfRepository
    {
        IQueryable<Game> Shelf { get; }

        IQueryable<Game> AvailableGames { get; }

        void AddToShelf(int GameId);

        GameAppUser RemoveFromShelf(int GameId);
    }
}