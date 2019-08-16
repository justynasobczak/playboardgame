using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public interface IMessageRepository
    {   
        IEnumerable<Message> GetMessagesForMeeting(int meetingId);

        void SaveMessage(Message message);

        Message DeleteMessage(int messageId);

        Message GetMessage(int messageId);
    }
}