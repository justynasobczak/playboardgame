using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IShelfRepository
    {
        IQueryable<Game> GetShelfForUser(string userId);

        IQueryable<Game> GetAvailableGamesForUser(string userId);

        void AddToShelf(GameAppUser gameAppUser);

        GameAppUser RemoveFromShelf(GameAppUser gameAppUser);
    }
}