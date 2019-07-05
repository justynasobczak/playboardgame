using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class MessagesListViewModel
    {
        public IEnumerable<Message> Messages { get; set; }
        
        public string Text { get; set; }
    }
}