using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public class Game
    {
        public int GameID { get; set; }

        public string Title { get; set; }

        public virtual ICollection<GameAppUser> GameAppUser { get; set; } = new List<GameAppUser>();
    }
}
