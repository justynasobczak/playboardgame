using System;

namespace PlayBoardGame.Models
{
    public interface ITomorrowsMeetingsNotificationRepository
    {
        void SaveNotification(TomorrowsMeetingsNotification notification);
        TomorrowsMeetingsNotification GetNotification(int meetingId, string userId, DateTime startDate);
    }
}