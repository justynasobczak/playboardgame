using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IGameRepository
    {
        IQueryable<Game> Games { get; }
    }
}
