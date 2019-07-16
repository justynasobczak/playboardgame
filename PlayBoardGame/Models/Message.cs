using System;

namespace PlayBoardGame.Models
{
    public class Message
    {
        public int MessageId { get; set; }

        public string Text { get; set; }
        
        public AppUser Author { get; set; }
        
        public string AuthorId { get; set; }
        
        public DateTime Created { get; set; }
        
        public Meeting Meeting { get; set; }
        
        public int? MeetingId { get; set; }
    }
}