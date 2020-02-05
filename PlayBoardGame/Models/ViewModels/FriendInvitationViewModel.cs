using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{
    public class FriendInvitationViewModel
    {
        public class SentInvitationsViewModel
        {
            public List<InvitationsListViewModel> InvitedUsersList { get; set; }

            [EmailAddress, MaxLength(256)]
            [Display(Name = "Invite your friend")]
            public string InvitedEmail { get; set; }
        }

        public class InvitationsListViewModel
        {
            public string PostDate { get; set; }
            
            public string UserName { get; set; }

            public string DisplayedUserName { get; set; }
            
            public string UserEmail { get; set; }

            public FriendInvitationStatus Status { get; set; }

            public int Id { get; set; }
        }
        
    }
}