using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class InvitedUserController : Controller
    {
        private readonly IInvitedUserRepository _invitedUserRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMeetingRepository _meetingRepository;

        public InvitedUserController(IInvitedUserRepository invitedUserRepository, UserManager<AppUser> userManager,
            IMeetingRepository meetingRepository)
        {
            _invitedUserRepository = invitedUserRepository;
            _userManager = userManager;
            _meetingRepository = meetingRepository;
        }

        public IActionResult List(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(MeetingController.List), "Meeting");
            }

            var invitedUsersList = new Dictionary<string, InvitationStatus>();
            invitedUsersList = _invitedUserRepository.GetInvitedUsersList(id);

            var meeting = _meetingRepository.GetMeeting(id);

            var list = new List<InvitedUserViewModel.InvitedUsersList>();

            foreach (var kvp in invitedUsersList)
            {
                var user = _userManager.FindByIdAsync(kvp.Key).Result;
                list.Add(new InvitedUserViewModel.InvitedUsersList
                {
                    DisplayedUserName = user.UserName + " " + user.FirstName + " " + user.LastName,
                    UserName = user.UserName,
                    Status = kvp.Value,
                    Id = kvp.Key
                });
            }

            var vm = new InvitedUserViewModel.InvitedUserListViewModel
            {
                InvitedUsers = _invitedUserRepository.GetInvitedUsers(id),
                MeetingId = id,
                AvailableUsers = _invitedUserRepository.GetAvailableUsers(id).ToList(),
                InvitedUsersList = list,
                IsEditable = meeting.Organizer.UserName == User.Identity.Name
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string userId, int meetingId)
        {
            var deletedEntry = _invitedUserRepository.RemoveUserFromMeeting(userId, meetingId);
            if (deletedEntry != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction(nameof(List), new {id = meetingId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(InvitedUserViewModel.InvitedUserListViewModel vm)
        {
            var userId = vm.SelectedToInviteUserId;
            var meetingId = vm.MeetingId;
            var meeting = _meetingRepository.GetMeeting(meetingId);
            var overlappingMeetings = new List<Meeting>();
            overlappingMeetings = _meetingRepository.GetOverlappingMeetingsForUser(meeting.StartDateTime, meeting.EndDateTime, userId)
                .ToList();
            var overlappingMeetingsTitle = " ";

            if (overlappingMeetings.Count > 0)
            {
                foreach (var m in overlappingMeetings)
                {
                    overlappingMeetingsTitle += m.Title + " ";
                }
                TempData["ErrorMessage"] = Constants.OverlappingMeetingsMessage + overlappingMeetingsTitle;
                return RedirectToAction(nameof(List), new {id = meetingId});
            }
            _invitedUserRepository.AddUserToMeeting(userId, meetingId, InvitationStatus.Pending);
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;

            return RedirectToAction(nameof(List), new {id = meetingId});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ChangeStatus(string userId, int meetingId, InvitationStatus status)
        {
            _invitedUserRepository.ChangeStatus(userId, meetingId, status);
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(List), new {id = meetingId});
        }
    }
}