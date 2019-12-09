using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class StartViewModels
    {
        public List<UpcomingMeetings> UpcomingMeetings { get; set; }
    }

    public class UpcomingMeetings
    {
        public string StartDate { get; set; }
        public string People { get; set; }
        public string Games { get; set; }
    }
}