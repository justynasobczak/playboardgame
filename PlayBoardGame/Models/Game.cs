using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public class Game
    {
        public int GameId { get; set; }

        public string Title { get; set; }

        public IEnumerable<GameAppUser> GameAppUser { get; set; }
        
        public IEnumerable<MeetingGame> MeetingGame { get; set; }
        
        public string PhotoPath { get; set; }
    }
}
