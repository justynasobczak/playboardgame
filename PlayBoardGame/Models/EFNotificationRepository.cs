using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFNotificationRepository : INotificationRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFNotificationRepository(ApplicationDBContext applicationDbContext)
        {
            _applicationDBContext = applicationDbContext;
        }

        public void SaveNotification(Notification notification)
        {
            if (notification.NotificationId == 0)
            {
                _applicationDBContext.Notifications.Add(notification);
            }
            else
            {
                var dbEntry =
                    _applicationDBContext.Notifications.FirstOrDefault(n =>
                        n.NotificationId == notification.NotificationId);
                if (dbEntry != null)
                {
                    dbEntry.Content = notification.Content;
                }
            }

            _applicationDBContext.SaveChanges();
        }
    }
}