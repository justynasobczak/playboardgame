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

        public IActionResult List(int meetingId)
        {
            if (meetingId == 0)
            {
                return RedirectToAction("List", "Meeting");
            }
            var invitedUsers = _invitedUserRepository.GetInvitedUsers(meetingId);
            return View(new InvitedUserViewModel.InvitedUserListViewModel
            {
                InvitedUsers = invitedUsers,
                MeetingId = meetingId
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

            return RedirectToAction("List", new { meetingId = meetingId });
        }
    }
}