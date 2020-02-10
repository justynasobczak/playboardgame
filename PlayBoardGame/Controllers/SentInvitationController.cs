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
                    DisplayedUserName = item.Invited != null
                        ? item.Invited.FullName
                        : Constants.NoAccountInvitationListMessage,
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
            if (!ModelState.IsValid) return RedirectToAction(nameof(List), new {vm.InvitedEmail});

            if (string.IsNullOrEmpty(vm.InvitedEmail))
            {
                TempData["ErrorMessage"] = Constants.EmptyEmailInvitationMessage;
                return RedirectToAction(nameof(List));
            }

            var user = _userManager.FindByEmailAsync(vm.InvitedEmail).Result;
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
            invitation.SenderId = GetCurrentUserId().Result;
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