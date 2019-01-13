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
    }
}
