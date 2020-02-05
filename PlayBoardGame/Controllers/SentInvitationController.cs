using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    public class SentInvitationController : Controller
    {
        private readonly IFriendInvitationRepository _friendInvitationRepository;
        private readonly UserManager<AppUser> _userManager;

        public SentInvitationController(IFriendInvitationRepository friendInvitationRepository,
            UserManager<AppUser> userManager)
        {
            _friendInvitationRepository = friendInvitationRepository;
            _userManager = userManager;
        }

        public IActionResult List(FriendInvitationViewModel.SentInvitationsViewModel vm)
        {
            var sentInvitationsList = _friendInvitationRepository
                .GetInvitationsSentByCurrentUser(GetCurrentUserId().Result)
                .Select(item => new FriendInvitationViewModel.InvitationsListViewModel()
                {
                    DisplayedUserName = item.Invited.FullName, UserName = item.Invited.UserName,
                    UserEmail = item.Invited.Email, Status = item.Status,
                    Id = item.FriendInvitationId
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
            if (ModelState.IsValid)
            {
                var friend = _userManager.FindByEmailAsync(vm.InvitedEmail).Result;
                var invitation = new FriendInvitation();
                if (friend != null)
                {
                    invitation.Invited = friend;
                }

                invitation.InvitedEmail = vm.InvitedEmail;
                invitation.SenderId = GetCurrentUserId().Result;
                _friendInvitationRepository.AddInvitation(invitation);
            }

            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(List));
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}