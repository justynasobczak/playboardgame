using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace PlayBoardGame.Models.ViewModels
{
    public class MessagesListViewModel
    {
        public IQueryable<Message> Messages { get; set; }

        [Required] public string Text { get; set; }
        
        public int MeetingId { get; set; }
    }

    public class EditMessageViewModel
    {
        [Required] public string Text { get; set; }

        public int MessageId { get; set; }
    }
}