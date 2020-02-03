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

        public IActionResult List()
        {
            var sentInvitationsList = _friendInvitationRepository
                .GetInvitationsSentByCurrentUser(GetCurrentUserId().Result)
                .Select(item => new InvitationsViewModel.InvitationsListViewModel()
                {
                    DisplayedUserName = item.Invited.FullName, UserName = item.Invited.UserName,
                    UserEmail = item.Invited.Email, Status = item.Status,
                    Id = item.FriendInvitationId
                })
                .ToList();

            var vm = new InvitationsViewModel.SentInvitationsViewModel
            {
                InvitedUsersList = sentInvitationsList
            };
            return View(vm);
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}