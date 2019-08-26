using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{
    public class MessagesListViewModel
    {
        public List<Message> Messages { get; set; }

        [Required] public string Text { get; set; }
        
        public int MeetingId { get; set; }
    }

    public class EditMessageViewModel
    {
        [Required] public string Text { get; set; }

        public int MessageId { get; set; }
    }
}