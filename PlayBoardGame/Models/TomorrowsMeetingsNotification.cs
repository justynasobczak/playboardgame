using System;

namespace PlayBoardGame.Models
{
    public class TomorrowsMeetingsNotification
    {
        public int TomorrowsMeetingsNotificationId { get; set; }

        public DateTime PostDate { get; set; }

        public DateTime MeetingStartDateTime { get; set; }
        
        public int MeetingId { get; set; }
        
        public Meeting Meeting { get; set; }

        public string ParticipantId { get; set; }

        public AppUser Participant { get; set; }

        public int NumberOfTries { get; set; }

        public bool IfSent { get; set; }
    }
}