using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFGameRepository : IGameRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFGameRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        // bozy Remember about paging to prevent fetching large number of records and causing memory usage bloating.
        //TODO paging
        public IEnumerable<Game> Games => _applicationDBContext.Games;

        public Game GetGame(int gameId) => _applicationDBContext.Games.FirstOrDefault(g => g.GameId == gameId);

        public void SaveGame(Game game)
        {
            if (game.GameId == 0)
            {
                _applicationDBContext.Games.Add(game);
            } else
            {
                var dbEntry = _applicationDBContext.Games.FirstOrDefault(g => g.GameId == game.GameId);
                if (dbEntry != null)
                {
                    dbEntry.Title = game.Title;
                    dbEntry.PhotoPath = game.PhotoPath;
                    dbEntry.PhotoName = game.PhotoName;
                }
            }
            _applicationDBContext.SaveChanges();

        }

        public Game DeleteGame(int GameID)
        {
            var dbEntry = _applicationDBContext.Games.FirstOrDefault(g => g.GameId == GameID);
            if (dbEntry == null) return dbEntry;
            _applicationDBContext.Games.Remove(dbEntry);
            _applicationDBContext.SaveChanges();
            return dbEntry;

        }
    }
}
