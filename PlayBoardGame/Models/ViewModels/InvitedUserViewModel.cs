using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class InvitedUserViewModel
    {
        public class InvitedUserListViewModel
        {
            public IEnumerable<AppUser> InvitedUsers { get; set; }
        }
    }
}