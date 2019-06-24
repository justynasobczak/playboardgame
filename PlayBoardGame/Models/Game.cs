using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public class Game
    {
        public int GameID { get; set; }

        public string Title { get; set; }

        public ICollection<GameAppUser> GameAppUser { get; } = new List<GameAppUser>();
        
        public IEnumerable<MeetingGame> MeetingGame { get; set; }
    }
}
