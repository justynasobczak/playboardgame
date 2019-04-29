using System;
using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public class Meeting
    {
        
        public int MeetingID { get; set; }

        public string Title { get; set; }
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
        
        public AppUser Organizer { get; set; }
        
        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }
        
        public ICollection<MeetingInvitedUser> MeetingInvitedUser { get; }
        
    }
}