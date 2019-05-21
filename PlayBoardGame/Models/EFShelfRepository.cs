using System.Linq;
using Microsoft.AspNetCore.Identity;
using PlayBoardGame.Infrastructure;

namespace PlayBoardGame.Models
{
    public class EFShelfRepository : IShelfRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly ContextProvider _contextProvider;

        public EFShelfRepository(ApplicationDBContext applicationDBContext, ContextProvider contextProvider)
        {
            _applicationDBContext = applicationDBContext;
            _contextProvider = contextProvider;
        }

        public IQueryable<Game> Shelf =>
            _applicationDBContext.Games.Where(g => g.GameAppUser.Any(gu => gu.AppUser.Id == _contextProvider.GetCurrentUserId().Result));

        public IQueryable<Game> AvailableGames => _applicationDBContext.Games.Except(Shelf);

        public void AddToShelf(int GameId)
        {
            var game = _applicationDBContext.Games.FirstOrDefault(g => g.GameID == GameId);

            var user = _contextProvider.GetCurrentUser().Result;

            _applicationDBContext.Set<GameAppUser>().Add(new GameAppUser
            {
                GameID = game.GameID,
                UserId = user.Id
            });

            _applicationDBContext.SaveChanges();
        }

        public GameAppUser RemoveFromShelf(int GameId)
        {
            var game = _applicationDBContext.Games.FirstOrDefault(g => g.GameID == GameId);

            var user = _contextProvider.GetCurrentUser().Result;

            var dbEntry = _applicationDBContext.GameAppUser.FirstOrDefault
                (gu => gu.GameID == game.GameID && gu.UserId == user.Id);
            if (dbEntry != null)
            {
                _applicationDBContext.Set<GameAppUser>().Remove(dbEntry);

                _applicationDBContext.SaveChanges();
            }
            return dbEntry;
        }
    }
}
