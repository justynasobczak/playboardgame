using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models.ViewModels
{
    public class InvitedUserViewModel
    {
        public class InvitedUserListViewModel
        {
            public int MeetingId { get; set; }

            public List<AppUser> AvailableUsers { get; set; }

            public string SelectedToInviteUserId { get; set; }

            public List<InvitedUsersList> InvitedUsersList { get; set; }

            public bool IsEditable { get; set; }
        }

        public class InvitedUsersList
        {
            public string UserName { get; set; }

            public string DisplayedUserName { get; set; }

            public InvitationStatus Status { get; set; }

            public string Id { get; set; }
        }
    }
}