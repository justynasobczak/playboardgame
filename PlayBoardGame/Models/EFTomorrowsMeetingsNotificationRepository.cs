using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class EFTomorrowsMeetingsNotificationRepository : ITomorrowsMeetingsNotificationRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFTomorrowsMeetingsNotificationRepository(ApplicationDBContext applicationDbContext)
        {
            _applicationDBContext = applicationDbContext;
        }

        public void SaveNotification(TomorrowsMeetingsNotification notification)
        {
            notification.PostDate = DateTime.UtcNow;
            if (notification.TomorrowsMeetingsNotificationId == 0)
            {
                notification.MeetingStartDateTime = notification.Meeting.StartDateTime;
                notification.NumberOfTries = 1;
                _applicationDBContext.TomorrowsMeetingsNotifications.Add(notification);
            }
            else
            {
                var dbEntry =
                    _applicationDBContext.TomorrowsMeetingsNotifications.FirstOrDefault(n =>
                        n.TomorrowsMeetingsNotificationId == notification.TomorrowsMeetingsNotificationId);
                if (dbEntry != null)
                {
                    dbEntry.IfSent = notification.IfSent;
                    dbEntry.NumberOfTries++;
                }
            }

            _applicationDBContext.SaveChanges();
        }

        public TomorrowsMeetingsNotification GetNotification(int meetingId, string userId, DateTime startDate)
        {
            return _applicationDBContext.TomorrowsMeetingsNotifications
                .Include(n => n.Meeting)
                .Include(n => n.Participant)
                .FirstOrDefault(n =>
                    n.MeetingId == meetingId && n.ParticipantId == userId && n.MeetingStartDateTime == startDate);
        }
    }
}