using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IShelfRepository
    {
        IEnumerable<Game> GetShelfForUser(string userId);

        IQueryable<Game> GetAvailableGamesForUser(string userId);

        void AddToShelf(GameAppUser gameAppUser);

        GameAppUser RemoveFromShelf(GameAppUser gameAppUser);
    }
}