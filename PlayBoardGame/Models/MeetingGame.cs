namespace PlayBoardGame.Models
{
    public class MeetingGame
    {   
        public int MeetingId { get; set; }
        
        public Meeting Meeting { get; set; }
        
        public int GameId { get; set; }
        
        public Game Game { get; set; }
    }
}