using System.ComponentModel.DataAnnotations;
namespace PlayBoardGame.Models
{
    public class Game
    {
        public int GameID { get; set; }
        [Required(ErrorMessage = "Please enter a game name")]
        public string Title { get; set; }
    }
}
