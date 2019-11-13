using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IGameRepository
    {
        IQueryable<Game> Games { get; }
        
        Game GetGame(int gameId);

        void SaveGame(Game game);

        Game DeleteGame(int GameID);
    }
}
