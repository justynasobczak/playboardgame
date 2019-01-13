using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFGameRepository : IGameRepository
    {
        private ApplicationDBContext _applicationDBContext;

        public EFGameRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }
        public IQueryable<Game> Games => _applicationDBContext.Games;

        public void SaveGame(Game game)
        {
            if (game.GameID == 0)
            {
                _applicationDBContext.Games.Add(game);
            } else
            {
                Game dbEntry = _applicationDBContext.Games.FirstOrDefault(g => g.GameID == game.GameID);
                if (dbEntry != null)
                {
                    dbEntry.Title = game.Title;
                }
            }
            _applicationDBContext.SaveChanges();

        }

        public Game DeleteGame(int GameID)
        {
            Game dbEntry = _applicationDBContext.Games.FirstOrDefault(g => g.GameID == GameID);
            if (dbEntry != null)
            {
                _applicationDBContext.Games.Remove(dbEntry);
                _applicationDBContext.SaveChanges();
            }
            return dbEntry;

        }
    }
}
