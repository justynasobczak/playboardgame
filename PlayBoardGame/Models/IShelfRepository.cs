using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public interface IShelfRepository
    {
        IEnumerable<Game> GetShelfForUser(string userId);

        IEnumerable<Game> GetAvailableGamesForUser(string userId);

        void AddToShelf(GameAppUser gameAppUser);

        GameAppUser RemoveFromShelf(GameAppUser gameAppUser);
    }
}