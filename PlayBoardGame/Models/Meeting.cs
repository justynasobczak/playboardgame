using System;

namespace PlayBoardGame.Models
{
    public class Meeting
    {
        
        public int MeetingID { get; set; }

        public string Title { get; set; }
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
        
        public AppUser Organizer { get; set; }
        
    }
}