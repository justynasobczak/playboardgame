using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class InvitedUserViewModel
    {
        public class InvitedUserListViewModel
        {
            public IEnumerable<AppUser> InvitedUsers { get; set; }
            
            public int MeetingId { get; set; }
            
            public List<AppUser> AvailableUsers { get; set; }
            
            public string SelectedToInviteUserId { get; set; }
        }
    }
}