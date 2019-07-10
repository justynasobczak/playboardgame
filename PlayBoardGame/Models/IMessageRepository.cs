using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMessageRepository
    {
        IQueryable<Message> Messages { get; }

        void SaveMessage(Message message);

        Message DeleteMessage(int messageId);

        Message GetMessage(int messageId);
    }
}