using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    public class InvitedUserController : Controller
    {
        private readonly IInvitedUserRepository _invitedUserRepository;
        private readonly UserManager<AppUser> _userManager;

        public InvitedUserController(IInvitedUserRepository invitedUserRepository, UserManager<AppUser> userManager)
        {
            _invitedUserRepository = invitedUserRepository;
            _userManager = userManager;
        }

        public IActionResult List(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("List", "Meeting");
            }
            var invitedUsers = _invitedUserRepository.GetInvitedUsers(id);
            var availableUsers = _invitedUserRepository.GetAvailableUsers(id);
            return View(new InvitedUserViewModel.InvitedUserListViewModel
            {
                InvitedUsers = invitedUsers,
                MeetingId = id,
                AvailableUsers = availableUsers.ToList()
            });
        }

        [HttpPost]
        public IActionResult Delete(string userId, int meetingId)
        {
            var deletedEntry = _invitedUserRepository.RemoveUserFromMeeting(userId, meetingId);
            if (deletedEntry != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction("List", new { id = meetingId });
        }

        [HttpPost]
        public IActionResult Add(InvitedUserViewModel.InvitedUserListViewModel vm)
        {
            var userId = vm.SelectedToInviteUserId;
            var meetingId = vm.MeetingId;
            _invitedUserRepository.AddUserToMeeting(userId, meetingId, false);
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction("List", new {id = meetingId});
        }
    }
}