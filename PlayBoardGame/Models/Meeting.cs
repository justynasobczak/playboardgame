using System;
using System.Collections.Generic;
using System.Net.Mime;

namespace PlayBoardGame.Models
{
    public class Meeting
    {
        public int MeetingID { get; set; }

        public string Title { get; set; }
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
        
        public string OrganizerId { get; set; }
        
        public AppUser Organizer { get; set; }
        
        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public Country Country { get; set; }
        
        public IEnumerable<MeetingInvitedUser> MeetingInvitedUser { get; set; }
        
        public string Notes { get; set; }
        
        public IEnumerable<MeetingGame> MeetingGame { get; set; }
        
    }
}