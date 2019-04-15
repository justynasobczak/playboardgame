using  System.Linq;

namespace PlayBoardGame.Models
{
    public interface IEventRepository
    {
        IQueryable<Event> Events { get; }
    }
}