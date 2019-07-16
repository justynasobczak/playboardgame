using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMessageRepository
    {
        IQueryable<Message> Messages { get; }
        
        IQueryable<Message> GetMessagesList(int meetingId);

        void SaveMessage(Message message);

        Message DeleteMessage(int messageId);

        Message GetMessage(int messageId);
    }
}