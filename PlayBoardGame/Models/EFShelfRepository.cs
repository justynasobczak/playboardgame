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

        public IQueryable<Game> GetShelfForUser(string userId)
        {
            return _applicationDBContext.Games.Where(g => g.GameAppUser.Any(gu => gu.AppUser.Id == userId));
        }

        public IQueryable<Game> GetAvailableGamesForUser(string userId)
        {
            var shelf = GetShelfForUser(userId);
            return _applicationDBContext.Games.Except(shelf);
        }

        public void AddToShelf(GameAppUser gameAppUser)
        {
            _applicationDBContext.Set<GameAppUser>().Add(gameAppUser);

            _applicationDBContext.SaveChanges();
        }

        public GameAppUser RemoveFromShelf(GameAppUser gameAppUser)
        {
            var dbEntry = _applicationDBContext.GameAppUser.FirstOrDefault
                (gu => gu.GameID == gameAppUser.GameID && gu.UserId == gameAppUser.UserId);
            if (dbEntry != null)
            {
                _applicationDBContext.Set<GameAppUser>().Remove(dbEntry);

                _applicationDBContext.SaveChanges();
            }

            return dbEntry;
        }
    }
}