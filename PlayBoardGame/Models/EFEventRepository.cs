using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFEventRepository : IEventRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFEventRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public IQueryable<Event> Events => _applicationDBContext.Events;
    }
}