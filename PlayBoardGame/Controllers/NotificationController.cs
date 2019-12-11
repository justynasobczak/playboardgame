using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class NotificationController : Controller
    {
        //private readonly IEmailTemplateSender _templateSender;
        private readonly INotificationRepository _notificationRepository;

        public NotificationController(INotificationRepository notificationRepository)
        {
            //_templateSender = templateSender;
            _notificationRepository = notificationRepository;
        }

        /*[HttpPost]
        public async void SendEmail()
        {
            var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
            {
                IsHTML = true,
                ToEmail = "test",
                Subject = "subject"
            }, "title", "content", "buttonText", "buttonUrl");
        }*/
        [HttpPost]
        public void AddNotification()
        {
            _notificationRepository.SaveNotification(new Notification() {Content = "test"});
        }
    }
}