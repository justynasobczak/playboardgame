namespace PlayBoardGame.Models
{
    public interface INotificationRepository
    {
        void SaveNotification(Notification notification);
    }
}