using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PlayBoardGame.Models.ViewModels
{
    public class FriendInvitationViewModel
    {
        public class SentInvitationsViewModel
        {
            public List<SentInvitationsList> InvitedUsersList { get; set; }

            [EmailAddress, MaxLength(256)]
            [Display(Name = "Invite your friend")]
            public string InvitedEmail { get; set; }
        }
        
        public class ReceivedInvitationsViewModel
        {
            public List<ReceivedInvitationsList> ReceivedInvitationsList { get; set; }
        }

        public class SentInvitationsList
        {
            public string PostDate { get; set; }

            public string DisplayedUserName { get; set; }

            public string InvitedEmail { get; set; }

            public FriendInvitationStatus Status { get; set; }
        }
        
        public class ReceivedInvitationsList
        {
            public string PostDate { get; set; }

            public string SenderUserName { get; set; }
            
            public string SenderEmail { get; set; }

            public FriendInvitationStatus Status { get; set; }
            
            public int InvitationId { get; set; }
        }
    }
}