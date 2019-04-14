using System;

namespace PlayBoardGame.Models
{
    public class Event
    {
        
        public int EventID { get; set; }

        public string Title { get; set; }
        
        public DateTime StartDateTime { get; set; }
        
        public DateTime EndDateTime { get; set; }
    }
}