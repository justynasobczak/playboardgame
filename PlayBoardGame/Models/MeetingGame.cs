namespace PlayBoardGame.Models
{
    public class MeetingGame
    {
        public int MeetingID { get; set; }
        
        public Meeting Meeting { get; set; }
        
        public int GameID { get; set; }
        
        public Game Game { get; set; }
    }
}