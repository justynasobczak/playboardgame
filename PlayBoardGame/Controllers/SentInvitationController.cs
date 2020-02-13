using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    public class SentInvitationController : Controller
    {
        private readonly IFriendInvitationRepository _friendInvitationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<SentInvitationController> _logger;

        public SentInvitationController(IFriendInvitationRepository friendInvitationRepository,
            UserManager<AppUser> userManager, ILogger<SentInvitationController> logger)
        {
            _friendInvitationRepository = friendInvitationRepository;
            _userManager = userManager;
            _logger = logger;
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
        public IActionResult Sent(FriendInvitationViewModel.SentInvitationsViewModel vm)
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
            
            var user = _userManager.FindByEmailAsync(vm.InvitedEmail).Result;
            var currentUserEmail = _userManager.FindByIdAsync(currentUserId).Result.Email;

            if (_friendInvitationRepository.IfInvitationWasReceivedByCurrentUser(user, currentUserEmail))
            {
                TempData["ErrorMessage"] = Constants.ExistingInvitationReceivedByCurrentUserErrorMessage;
                return RedirectToAction(nameof(List), new {vm.InvitedEmail});
            }
            
            var invitation = new FriendInvitation();
            if (user != null)
            {
                invitation.Invited = user;
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

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}