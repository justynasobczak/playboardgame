using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFShelfRepository : IShelfRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFShelfRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public IEnumerable<Game> GetShelfForUser(string userId)
        {
            return _applicationDBContext.Games.Where(g => g.GameAppUser.Any(gu => gu.AppUser.Id == userId));
        }

        public IQueryable<Game> GetAvailableGamesForUser(string userId)
        {
            return _applicationDBContext.Games.Where(g => g.GameAppUser.All(gu => gu.AppUser.Id != userId));
        }

        public void AddToShelf(GameAppUser gameAppUser)
        {
            _applicationDBContext.GameAppUser.Add(gameAppUser);

            _applicationDBContext.SaveChanges();
        }

        public GameAppUser RemoveFromShelf(GameAppUser gameAppUser)
        {
            var dbEntry = _applicationDBContext.GameAppUser.FirstOrDefault
                (gu => gu.GameId == gameAppUser.GameId && gu.UserId == gameAppUser.UserId);
            if (dbEntry != null)
            {
                _applicationDBContext.GameAppUser.Remove(dbEntry);

                _applicationDBContext.SaveChanges();
            }

            return dbEntry;
        }
    }
}