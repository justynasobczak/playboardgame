using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class TomorrowsMeetingsNotificationController : Controller
    {
        private readonly ITomorrowsMeetingsNotificationRepository _notificationRepository;
        private readonly IMeetingRepository _meetingRepository;

        public TomorrowsMeetingsNotificationController(ITomorrowsMeetingsNotificationRepository notificationRepository,
            IMeetingRepository meetingRepository)
        {
            _notificationRepository = notificationRepository;
            _meetingRepository = meetingRepository;
        }

        [HttpPost]
        public void SendNotification()
        {
            var usersToSendNotification = _meetingRepository.GetUsersToSendNotification();
            //var usersToSendAgainNotification = _meetingRepository.GetUsersToSendAgainNotification();
            foreach (var item in usersToSendNotification)
            {
                //send email
               /* _notificationRepository.SaveNotification(new TomorrowsMeetingsNotification()
                {
                    Meeting = item.Key,
                    Participant = item.Value,
                    IfSent = true,
                    MeetingStartDateTime = item.Key.StartDateTime,
                    NumberOfTries = 1,
                });*/
            }
        }
    }
}