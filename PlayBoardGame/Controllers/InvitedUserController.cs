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

        public ViewResult List(int meetingId)
        {
            var invitedUsers = _invitedUserRepository.GetInvitedUsers(meetingId);
            return View(new InvitedUserViewModel.InvitedUserListViewModel
            {
                InvitedUsers = invitedUsers
            });
        }
    }
}