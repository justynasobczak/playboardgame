using System.Globalization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Infrastructure;
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
        private readonly UserManager<AppUser> _userManager;

        public TomorrowsMeetingsNotificationController(ITomorrowsMeetingsNotificationRepository notificationRepository,
            IMeetingRepository meetingRepository, IEmailTemplateSender templateSender,
            ILogger<TomorrowsMeetingsNotificationController> logger, UserManager<AppUser> userManager)
        {
            _notificationRepository = notificationRepository;
            _meetingRepository = meetingRepository;
            _templateSender = templateSender;
            _logger = logger;
            _userManager = userManager;
        }

        [HttpPost]
        public void SendNotification()
        {
            var usersToSendNotification = _meetingRepository.GetUsersToSendNotification();
            foreach (var item in usersToSendNotification)
            {
                var appLink = Url.Action("Edit", "Meeting", new {id = item.Meeting.MeetingId},
                    HttpContext.Request.Scheme);
                var timeZone = _userManager.FindByIdAsync(item.User.Id).Result.TimeZone;
                var StartDateTime = ToolsExtensions
                    .ConvertToTimeZoneFromUtc(item.Meeting.StartDateTime, timeZone, _logger)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);
                var content = $"Start date: {StartDateTime}, Organizer: {item.User.FullName}.";
                var result = _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                    {
                        IsHTML = true,
                        ToEmail = item.User.Email,
                        Subject = Constants.SubjectTomorrowsMeetingEmail
                    }, Constants.TitleTomorrowsMeetingEmail,
                    $"{Constants.ContentTomorrowsMeetingEmail}{content}",
                    Constants.ButtonCheckMeeting,
                    appLink);

                //Async?
                var notification = _notificationRepository.GetNotification(item.Meeting.MeetingId,
                                       item.User.Id, item.Meeting.StartDateTime) ??
                                   new TomorrowsMeetingsNotification();
                notification.Meeting = item.Meeting;
                notification.Participant = item.User;
                notification.IfSent = result.Result.Successful;

                _notificationRepository.SaveNotification(notification);
            }
        }
    }
}