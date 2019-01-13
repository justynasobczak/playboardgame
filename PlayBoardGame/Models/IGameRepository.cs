using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IGameRepository
    {
        IQueryable<Game> Games { get; }

        void SaveGame(Game game);

        Game DeleteGame(int GameID);
    }
}
