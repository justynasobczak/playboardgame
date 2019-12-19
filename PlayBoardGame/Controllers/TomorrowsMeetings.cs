using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class TomorrowsMeetings : Controller
    {
        private readonly INotificationRepository _notificationRepository;

        public TomorrowsMeetings(INotificationRepository notificationRepository)
        {
            _notificationRepository = notificationRepository;
        }

        [HttpPost]
        public void AddNotification()
        {
            _notificationRepository.SaveNotification(new Notification() {Content = "test"});
        }
    }
}