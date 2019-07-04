using System.Collections.Generic;

namespace PlayBoardGame.Models
{
    public interface IMessageRepository
    {
        IEnumerable<Message> Messages { get; }

        void SaveMessage(Message message);

        Message DeleteMessage(int messageId);

    }
}