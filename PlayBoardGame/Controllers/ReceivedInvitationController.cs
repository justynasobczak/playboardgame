using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    public class ReceivedInvitationController : Controller
    {
        private readonly IFriendInvitationRepository _friendInvitationRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<ReceivedInvitationController> _logger;

        public ReceivedInvitationController(IFriendInvitationRepository friendInvitationRepository,
            UserManager<AppUser> userManager, ILogger<ReceivedInvitationController> logger)
        {
            _friendInvitationRepository = friendInvitationRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult List()
        {
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
        public IActionResult ChangeStatus(int invitationId, FriendInvitationStatus status)
        {
            _friendInvitationRepository.ChangeStatus(invitationId, status);
            return RedirectToAction(nameof(List));
        }
    }
}