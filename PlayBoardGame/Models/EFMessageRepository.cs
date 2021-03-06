using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class EFMessageRepository : IMessageRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFMessageRepository(ApplicationDBContext applicationDbContext)
        {
            _applicationDBContext = applicationDbContext;
        }

        public IEnumerable<Message> GetMessagesForMeeting(int meetingId)
        {
            return _applicationDBContext.Messages
                .Include(m => m.Author).Where(m => m.MeetingId == meetingId);
        }

        public void SaveMessage(Message message)
        {
            if (message.MessageId == 0)
            {
                _applicationDBContext.Messages.Add(message);
            }
            else
            {
                var dbEntry = _applicationDBContext.Messages.FirstOrDefault(m => m.MessageId == message.MessageId);
                if (dbEntry != null)
                {
                    dbEntry.Text = message.Text;
                }
            }

            _applicationDBContext.SaveChanges();
        }

        public Message GetMessage(int messageId)
        {
            return _applicationDBContext.Messages.FirstOrDefault(m => m.MessageId == messageId);
        }

        public Message DeleteMessage(int messageId)
        {
            var dbEntry = _applicationDBContext.Messages.FirstOrDefault(m => m.MessageId == messageId);
            if (dbEntry != null)
            {
                _applicationDBContext.Messages.Remove(dbEntry);
            }

            _applicationDBContext.SaveChanges();
            return dbEntry;
        }
    }
}