﻿using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models.ViewModels
{
    public class ShelfListViewModel
    {
        public List<Game> Shelf { get; set; }
    }

    public class ShelfEditViewModel
    {
        public List<Game> Shelf { get; set; }
        public List<Game> AvailableGames { get; set; }
    }

    public class ShelfModificationViewModel
    {
        public IEnumerable<int> IdsToAdd { get; } = new List<int>();
        public IEnumerable<int> IdsToDelete { get; } = new List<int>();
    }
}