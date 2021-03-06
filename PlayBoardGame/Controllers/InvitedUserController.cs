using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class InvitedUserController : Controller
    {
        private readonly IInvitedUserRepository _invitedUserRepository;
        private readonly IMeetingRepository _meetingRepository;
        private readonly IFriendInvitationRepository _friendInvitationRepository;
        private readonly IEmailTemplateSender _templateSender;
        private readonly ILogger<InvitedUserController> _logger;
        private readonly UserManager<AppUser> _userManager;

        public InvitedUserController(IInvitedUserRepository invitedUserRepository, IMeetingRepository meetingRepository,
            IEmailTemplateSender templateSender, ILogger<InvitedUserController> logger,
            UserManager<AppUser> userManager, IFriendInvitationRepository friendInvitationRepository)
        {
            _invitedUserRepository = invitedUserRepository;
            _meetingRepository = meetingRepository;
            _templateSender = templateSender;
            _logger = logger;
            _userManager = userManager;
            _friendInvitationRepository = friendInvitationRepository;
        }

        public IActionResult List(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(MeetingController.List), "Meeting");
            }

            var invitedUsersList = _invitedUserRepository.GetInvitedUsersList(id)
                .Select(item => new InvitedUserViewModel.InvitedUsersList
                {
                    DisplayedUserName = item.AppUser.FullName, UserName = item.AppUser.UserName,
                    UserEmail = item.AppUser.Email, Status = item.Status,
                    Id = item.AppUser.Id
                })
                .ToList();

            var meeting = _meetingRepository.GetMeeting(id);
            var vm = new InvitedUserViewModel.InvitedUserListViewModel
            {
                MeetingId = id,
                /*AvailableUsers = _invitedUserRepository.GetAvailableUsers(id)
                    .OrderBy(u => u.LastName).ThenBy(u => u.Email)
                    .ToList(),*/
                AvailableUsers =
                    _friendInvitationRepository.GetFriendsOfCurrentUser(GetCurrentUserId().Result)
                        .OrderBy(u => u.LastName).ThenBy(u => u.Email).ToList(),
                InvitedUsersList = invitedUsersList,
                IsEditable = meeting.Organizer.UserName == User.Identity.Name
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string userId, int meetingId)
        {
            var deletedEntry = _invitedUserRepository.RemoveUserFromMeeting(userId, meetingId);
            if (deletedEntry == null) return RedirectToAction(nameof(List), new {id = meetingId});
            var user = _userManager.FindByIdAsync(userId).Result;
            var meeting = _meetingRepository.GetMeeting(meetingId);
            var appLink = Url.Action("Login", "Account", null, HttpContext.Request.Scheme);
            var content =
                $"{Constants.ContentDeleteInvitationEmail}: Meeting title: {meeting.Title}, Organizer: {meeting.Organizer.FullName}";
            var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                {
                    IsHTML = true,
                    ToEmail = user.Email,
                    Subject = Constants.SubjectDeleteInvitationEmail
                }, Constants.TitleDeleteInvitationEmail, content,
                Constants.ButtonVisitSide,
                appLink);

            if (response.Successful)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction(nameof(List), new {id = meetingId});
            }

            TempData["ErrorMessage"] = Constants.GeneralSendEmailErrorMessage;
            foreach (var error in response.Errors)
            {
                _logger.LogError(error);
            }

            return RedirectToAction(nameof(List), new {id = meetingId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(InvitedUserViewModel.InvitedUserListViewModel vm)
        {
            var userId = vm.SelectedToInviteUserId;
            var meetingId = vm.MeetingId;
            if (_invitedUserRepository.IfUserIsInvited(meetingId, userId))
            {
                TempData["ErrorMessage"] = Constants.ExistingMeetingInvitationMessage;
                return RedirectToAction(nameof(List), new {id = meetingId});
            }

            var meeting = _meetingRepository.GetMeeting(meetingId);
            var user = _userManager.FindByIdAsync(userId).Result;
            var games = meeting.MeetingGame.Select(mg => mg.Game.Title);
            var meetingStartDate = ToolsExtensions
                .ConvertToTimeZoneFromUtc(meeting.StartDateTime, user.TimeZone, _logger)
                .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);
            var meetingEndDate = ToolsExtensions.ConvertToTimeZoneFromUtc(meeting.EndDateTime, user.TimeZone, _logger)
                .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture);

            var overlappingMeetings = new List<string>();
            overlappingMeetings = _meetingRepository
                .GetOverlappingMeetingsForUser(meeting.StartDateTime, meeting.EndDateTime, userId)
                .Select(m => m.Title)
                .ToList();
            // bozy This variable should be inside the if part. Moreover setting a variable with a single space is a code smell

            if (overlappingMeetings.Count > 0)
            {
                // use string.Join() instead of foreach.
                var overlappingMeetingsTitle = string.Join(", ", overlappingMeetings);

                TempData["ErrorMessage"] = $"{Constants.OverlappingMeetingsMessage}: {overlappingMeetingsTitle}";
                return RedirectToAction(nameof(List), new {id = meetingId});
            }

            _invitedUserRepository.AddUserToMeeting(userId, meetingId, InvitationStatus.Pending);
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;

            var appLink = Url.Action(nameof(List), "InvitedUser", new {id = meetingId}, HttpContext.Request.Scheme);
            var content =
                $"{Constants.ContentInviteUserEmail}: Organizer: {meeting.Organizer.FullName}; Start date: {meetingStartDate}; End date: {meetingEndDate};" +
                $" Games: {string.Join(", ", games)}";
            var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                {
                    IsHTML = true,
                    ToEmail = user.Email,
                    Subject = Constants.SubjectInviteUserEmail
                }, Constants.TitleInviteUserEmail, content,
                Constants.ButtonCheckMeeting,
                appLink);

            if (response.Successful) return RedirectToAction(nameof(List), new {id = meetingId});
            TempData["ErrorMessage"] = Constants.GeneralSendEmailErrorMessage;
            foreach (var error in response.Errors)
            {
                _logger.LogError(error);
            }

            return RedirectToAction(nameof(List), new {id = meetingId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(string userId, int meetingId, InvitationStatus status)
        {
            _invitedUserRepository.ChangeStatus(userId, meetingId, status);

            var appLink = Url.Action(nameof(List), "InvitedUser", new {id = meetingId}, HttpContext.Request.Scheme);
            var meeting = _meetingRepository.GetMeeting(meetingId);
            var user = _userManager.FindByIdAsync(userId).Result;
            var content = $"meeting: {meeting.Title}, new status: {status} changed by the user: {user.FullName}";
            var users = _invitedUserRepository.GetUsersEmailsForNotification(meetingId,
                _userManager.FindByIdAsync(userId).Result.Id);
            foreach (var email in users)
            {
                _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                        {
                            IsHTML = true,
                            ToEmail = email,
                            Subject = Constants.SubjectNewStatusInvitationEmail
                        }, Constants.TitleNewStatusInvitationEmail,
                        $"{Constants.ContentNewStatusInvitationEmail}: {content}",
                        Constants.ButtonCheckMeeting,
                        appLink)
                    .ContinueWith(t =>
                    {
                        if (t.Result.Successful) return;
                        foreach (var error in t.Result.Errors)
                        {
                            _logger.LogError(error);
                        }
                    }, TaskScheduler.Default);
            }

            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(List), new {id = meetingId});
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}