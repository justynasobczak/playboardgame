using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Scaffolding.Internal;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Migrations;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class InvitedUserController : Controller
    {
        private readonly IInvitedUserRepository _invitedUserRepository;
        private readonly IMeetingRepository _meetingRepository;

        public InvitedUserController(IInvitedUserRepository invitedUserRepository, IMeetingRepository meetingRepository)
        {
            _invitedUserRepository = invitedUserRepository;
            _meetingRepository = meetingRepository;
        }

        public IActionResult List(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(MeetingController.List), "Meeting");
            }

            var invitedUsersList = new List<MeetingInvitedUser>();
            invitedUsersList = _invitedUserRepository.GetInvitedUsersList(id).ToList();

            var list = new List<InvitedUserViewModel.InvitedUsersList>();

            foreach (var item in invitedUsersList)
            {
                list.Add(new InvitedUserViewModel.InvitedUsersList
                {
                    // bozy Use string interpolation
                    // DisplayedUserName = $"{user.UserName} {user.FirstName} {user.LastName}";
                    //Changed
                    DisplayedUserName = $"{item.AppUser.UserName} {item.AppUser.FirstName} {item.AppUser.LastName}",
                    UserName = item.AppUser.UserName,
                    Status = item.Status,
                    Id = item.AppUser.Id
                });
            }

            var meeting = _meetingRepository.GetMeeting(id);
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
            var overlappingMeetings = new List<string>();
            overlappingMeetings = _meetingRepository
                .GetOverlappingMeetingsForUser(meeting.StartDateTime, meeting.EndDateTime, userId)
                .Select(m => m.Title)
                .ToList();
            // bozy This variable should be inside the if part. Moreover setting a variable with a single space is a code smell

            if (overlappingMeetings.Count > 0)
            {
                // use string.Join() instead of foreach.
                var overlappingMeetingsTitle = string.Join(", ", overlappingMeetings);
                
                TempData["ErrorMessage"] = $"{Constants.OverlappingMeetingsMessage}: {overlappingMeetingsTitle}";
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