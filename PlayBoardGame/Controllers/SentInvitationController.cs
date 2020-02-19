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
    public class SentInvitationController : Controller
    {
        private readonly IFriendInvitationRepository _friendInvitationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<SentInvitationController> _logger;
        private readonly IEmailTemplateSender _templateSender;

        public SentInvitationController(IFriendInvitationRepository friendInvitationRepository,
            UserManager<AppUser> userManager, ILogger<SentInvitationController> logger,
            IEmailTemplateSender templateSender)
        {
            _friendInvitationRepository = friendInvitationRepository;
            _userManager = userManager;
            _logger = logger;
            _templateSender = templateSender;
        }

        public IActionResult List(FriendInvitationViewModel.SentInvitationsViewModel vm)
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var timeZone = currentUser.TimeZone;
            var sentInvitationsList = _friendInvitationRepository
                .GetInvitationsSentByCurrentUser(currentUser.Id)
                .Select(item => new FriendInvitationViewModel.SentInvitationsList()
                {
                    InvitedUser = item.Invited != null
                        ? item.Invited.FullName
                        : item.InvitedEmail,
                    InvitedEmail = item.InvitedEmail,
                    Status = item.Status,
                    PostDate = ToolsExtensions.ConvertToTimeZoneFromUtc(item.PostDateTime, timeZone, _logger)
                        .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture)
                })
                .ToList();

            vm.InvitedUsersList = sentInvitationsList;
            return View(vm);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public async Task<IActionResult> Sent(FriendInvitationViewModel.SentInvitationsViewModel vm)
        {
            if (string.IsNullOrEmpty(vm.InvitedEmail))
            {
                TempData["ErrorMessage"] = Constants.EmptyEmailInvitationMessage;
                return RedirectToAction(nameof(List));
            }

            if (!ModelState.IsValid) return RedirectToAction(nameof(List), new {vm.InvitedEmail});

            var currentUserId = GetCurrentUserId().Result;

            if (_friendInvitationRepository.IfInvitationWasSentByCurrentUser(currentUserId, vm.InvitedEmail))
            {
                TempData["ErrorMessage"] = Constants.ExistingInvitationSentByCurrentUserErrorMessage;
                return RedirectToAction(nameof(List), new {vm.InvitedEmail});
            }

            var invitedUser = _userManager.FindByEmailAsync(vm.InvitedEmail).Result;
            var currentUserEmail = _userManager.FindByIdAsync(currentUserId).Result.Email;

            if (invitedUser != null &&
                _friendInvitationRepository.IfInvitationWasReceivedByCurrentUser(invitedUser.Id, currentUserEmail))
            {
                TempData["ErrorMessage"] = Constants.ExistingInvitationReceivedByCurrentUserErrorMessage;
                return RedirectToAction(nameof(List), new {vm.InvitedEmail});
            }

            var appLinkRegister = Url.Action("Register", "Account", new {Email = vm.InvitedEmail},
                HttpContext.Request.Scheme);
            var appLinkInvitations = Url.Action("List", "ReceivedInvitation", null, HttpContext.Request.Scheme);
            var response = await _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                {
                    IsHTML = true,
                    ToEmail = vm.InvitedEmail,
                    Subject = Constants.SubjectNewFriendInvitationEmail
                }, Constants.TitleNewFriendInvitationEmail,
                invitedUser != null
                    ? Constants.ContentNewFriendInvitationExistingUserEmail
                    : Constants.ContentNewFriendInvitationNonExistingUserEmail,
                invitedUser != null ? Constants.ButtonCheckFriendInvitation : Constants.ButtonVisitSide,
                invitedUser != null ? appLinkInvitations : appLinkRegister);
            if (response.Successful)
            {
                var invitation = new FriendInvitation();
                if (invitedUser != null)
                {
                    invitation.Invited = invitedUser;
                    TempData["SuccessMessage"] = Constants.ExistingAccountSentInvitationMessage;
                }
                else
                {
                    TempData["SuccessMessage"] = Constants.NoAccountSentInvitationMessage;
                }

                invitation.InvitedEmail = vm.InvitedEmail;
                invitation.SenderId = currentUserId;
                _friendInvitationRepository.AddInvitation(invitation);

                return RedirectToAction(nameof(List));
            }

            _logger.LogCritical(vm.InvitedEmail);
            TempData["ErrorMessage"] =
                $"{Constants.GeneralSendEmailErrorMessage} {Constants.FriendInvitationEmailErrorMessage}";
            foreach (var error in response.Errors)
            {
                _logger.LogError(error);
            }

            return RedirectToAction(nameof(List), new {vm.InvitedEmail});
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}