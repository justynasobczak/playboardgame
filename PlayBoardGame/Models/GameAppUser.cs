namespace PlayBoardGame.Models
{
    public class GameAppUser
    {
        public int GameID { get; set; }

        public Game Game { get; set; }

        public string UserId { get; set; }

        public AppUser AppUser { get; set; }
    }

}