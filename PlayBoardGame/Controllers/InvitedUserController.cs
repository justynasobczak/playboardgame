using System.Collections.Generic;
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

            var invitedUsersList = new Dictionary<string, bool>();
            invitedUsersList = _invitedUserRepository.GetInvitedUsersList(id);

            var list = new List<InvitedUserViewModel.InvitedUsersList>();

            foreach (var kvp in invitedUsersList)
            {
                var user = _userManager.FindByIdAsync(kvp.Key).Result;
                list.Add(new InvitedUserViewModel.InvitedUsersList
                {
                    UserName = user.UserName + " " + user.FirstName + " " + user.LastName,
                    IsAccepted = kvp.Value,
                    Id = kvp.Key
                });
            }

            var vm = new InvitedUserViewModel.InvitedUserListViewModel
            {
                InvitedUsers = _invitedUserRepository.GetInvitedUsers(id),
                MeetingId = id,
                AvailableUsers = _invitedUserRepository.GetAvailableUsers(id).ToList(),
                InvitedUsersList = list
            };
            return View(vm);
        }

        [HttpPost]
        public IActionResult Delete(string userId, int meetingId)
        {
            var deletedEntry = _invitedUserRepository.RemoveUserFromMeeting(userId, meetingId);
            if (deletedEntry != null)
            {
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            }

            return RedirectToAction("List", new {id = meetingId});
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

        [HttpPost]
        public IActionResult SetIsAccepted(string userId, int meetingId)
        {
            _invitedUserRepository.ChangeIsAccepted(userId, meetingId);
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction("List", new {id = meetingId});
        }
    }
}