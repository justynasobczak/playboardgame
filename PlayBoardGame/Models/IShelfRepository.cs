using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlayBoardGame.Models
{
    public interface IShelfRepository
    {
        IQueryable<Game> Shelf { get; }

        IQueryable<Game> AvailableGames { get; }

        void SaveShelf(int GameId);
    }
}