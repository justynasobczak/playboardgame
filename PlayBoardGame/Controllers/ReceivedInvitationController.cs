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
    public class ReceivedInvitationController : Controller
    {
        private readonly IFriendInvitationRepository _friendInvitationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ReceivedInvitationController> _logger;
        private readonly IEmailTemplateSender _templateSender;

        public ReceivedInvitationController(IFriendInvitationRepository friendInvitationRepository,
            UserManager<AppUser> userManager, ILogger<ReceivedInvitationController> logger,
            IEmailTemplateSender templateSender)
        {
            _friendInvitationRepository = friendInvitationRepository;
            _userManager = userManager;
            _logger = logger;
            _templateSender = templateSender;
        }

        public IActionResult List()
        {
            //TODO sort by status, PEnding first, then sort by Date
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var timeZone = currentUser.TimeZone;
            var receivedInvitationsList = _friendInvitationRepository
                .GetInvitationsReceivedByCurrentUser(currentUser.Email)
                .Select(item => new FriendInvitationViewModel.ReceivedInvitationsList
                {
                    SenderUserName = item.Sender.FullName,
                    SenderEmail = item.Sender.Email,
                    Status = item.Status,
                    PostDate = ToolsExtensions.ConvertToTimeZoneFromUtc(item.PostDateTime, timeZone, _logger)
                        .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                    InvitationId = item.FriendInvitationId
                })
                .ToList();

            var vm = new FriendInvitationViewModel.ReceivedInvitationsViewModel
            {
                ReceivedInvitationsList = receivedInvitationsList
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int invitationId, FriendInvitationStatus status)
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            _friendInvitationRepository.ChangeStatus(invitationId, status, currentUser);
            var invitation = _friendInvitationRepository.GetInvitation(invitationId);
            var appLink = Url.Action(nameof(List), "ReceivedInvitation", null, HttpContext.Request.Scheme);
            var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                {
                    IsHTML = true,
                    ToEmail = invitation.Sender.Email,
                    Subject = Constants.SubjectNewStatusInvitationEmail
                }, status == FriendInvitationStatus.Accepted
                    ? Constants.TitleFriendInvitationAcceptanceEmail
                    : Constants.TitleFriendInvitationRejectionEmail,
                status == FriendInvitationStatus.Accepted
                    ? $"{Constants.ContentFriendInvitationAcceptanceEmail}{invitation.Invited.FullName}"
                    : $"{Constants.ContentFriendInvitationRejectionEmail}{invitation.Invited.FullName}",
                Constants.ButtonCheckFriendInvitation,
                appLink);

            if (response.Successful)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction(nameof(List));
            }

            foreach (var error in response.Errors)
            {
                _logger.LogError(error);
            }

            TempData["ErrorMessage"] = Constants.GeneralSendEmailErrorMessage;
            return RedirectToAction(nameof(List));
        }
    }
}