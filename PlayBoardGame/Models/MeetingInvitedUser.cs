namespace PlayBoardGame.Models
{
    public class MeetingInvitedUser
    {
        public int MeetingID { get; set; }

        public Meeting Meeting { get; set; }

        public string UserId { get; set; }

        public AppUser AppUser { get; set; }
        
        public bool IsAccepted { get; set; }
    }
}