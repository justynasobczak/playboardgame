using System;
using System.Collections.Generic;
using System.Linq;

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
            if (notification.TomorrowsMeetingsNotificationId == 0)
            {
                _applicationDBContext.TomorrowsMeetingsNotifications.Add(notification);
            }
            else
            {
                var dbEntry =
                    _applicationDBContext.TomorrowsMeetingsNotifications.FirstOrDefault(n =>
                        n.TomorrowsMeetingsNotificationId == notification.TomorrowsMeetingsNotificationId);
                if (dbEntry != null)
                {
                    dbEntry.NumberOfTries++;
                    dbEntry.PostDate = DateTime.UtcNow;
                }
            }

            _applicationDBContext.SaveChanges();
        }
    }
}