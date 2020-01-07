using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class TomorrowsMeetingsNotificationController : Controller
    {
        private readonly ITomorrowsMeetingsNotificationRepository _notificationRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IEmailTemplateSender _templateSender;
        private readonly ILogger<TomorrowsMeetingsNotificationController> _logger;

        public TomorrowsMeetingsNotificationController(ITomorrowsMeetingsNotificationRepository notificationRepository,
            IMeetingRepository meetingRepository, IEmailTemplateSender templateSender,
            ILogger<TomorrowsMeetingsNotificationController> logger)
        {
            _notificationRepository = notificationRepository;
            _meetingRepository = meetingRepository;
            _templateSender = templateSender;
            _logger = logger;
        }

        [HttpPost]
        public void SendNotification()
        {
            var usersToSendNotification = _meetingRepository.GetUsersToSendNotification();
            foreach (var item in usersToSendNotification)
            {
                var appLink = Url.Action("Edit", "Meeting", new {id = item.Meeting.MeetingId},
                    HttpContext.Request.Scheme);
                //TODO: time zone
                var content = $"$Start date: {item.Meeting.StartDateTime}, Organizer: {item.User.FullName}";
                _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                        {
                            IsHTML = true,
                            ToEmail = item.User.Email,
                            Subject = Constants.SubjectTomorrowsMeetingEmail
                        }, Constants.TitleTomorrowsMeetingEmail,
                        $"{Constants.ContentTomorrowsMeetingEmail}{content}",
                        Constants.ButtonCheckMeeting,
                        appLink)
                    .ContinueWith(t =>
                    {
                        var notification = _notificationRepository.GetNotification(item.Meeting.MeetingId,
                                               item.User.Id, item.Meeting.StartDateTime) ??
                                           new TomorrowsMeetingsNotification();
                        notification.Meeting = item.Meeting;
                        notification.Participant = item.User;
                        notification.MeetingStartDateTime = item.Meeting.StartDateTime;
                        notification.IfSent = t.Result.Successful;

                        _notificationRepository.SaveNotification(notification);

                        foreach (var error in t.Result.Errors)
                        {
                            _logger.LogError(error);
                        }
                    }, TaskScheduler.Default);
            }
        }
    }
}