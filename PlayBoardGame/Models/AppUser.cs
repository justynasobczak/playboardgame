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

        public ICollection<GameAppUser> GameAppUser { get; } = new List<GameAppUser>();
        
        public ICollection<Meeting> OrganizedMeetings { get; set; }
        
        public ICollection<MeetingInvitedUser> MeetingInvitedUser { get; set; }
    }
}
