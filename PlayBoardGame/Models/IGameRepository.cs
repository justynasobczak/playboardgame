using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public interface IGameRepository
    {
        IEnumerable<Game> Games { get; }
        
        Game GetGame(int gameId);

        void SaveGame(Game game);

        Game DeleteGame(int GameID);
    }
}
