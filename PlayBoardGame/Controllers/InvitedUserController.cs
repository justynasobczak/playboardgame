using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    public class InvitedUserController : Controller
    {
        private readonly IInvitedUserRepository _invitedUserRepository;

        public InvitedUserController(IInvitedUserRepository invitedUserRepository)
        {
            _invitedUserRepository = invitedUserRepository;
        }

        public IActionResult List(int id)
        {
            if (id == 0)
            {
                return RedirectToAction("List", "Meeting");
            }
            var invitedUsers = _invitedUserRepository.GetInvitedUsers(id);
            return View(new InvitedUserViewModel.InvitedUserListViewModel
            {
                InvitedUsers = invitedUsers,
                MeetingId = id
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
    }
}