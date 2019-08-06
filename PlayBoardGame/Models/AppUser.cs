using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace PlayBoardGame.Models
{
    public class AppUser : IdentityUser

    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Street { get; set; }

        public string City { get; set; }

        public string PostalCode { get; set; }

        public string Country { get; set; }
        
        public string TimeZone { get; set; }

        public IEnumerable<GameAppUser> GameAppUser { get; set; }
        
        public IEnumerable<Meeting> OrganizedMeetings { get; set; }
        
        public IEnumerable<Message> WrittenMessages { get; set; }
        
        public IEnumerable<MeetingInvitedUser> MeetingInvitedUser { get; set; }
    }
}
