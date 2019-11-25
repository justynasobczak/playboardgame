using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IGameRepository
    {
        IQueryable<Game> Games { get; }
        
        Game GetGame(int gameId);

        bool IsGameInMeeting(int gameId);

        void SaveGame(Game game);

        Game DeleteGame(int GameID);
    }
}
