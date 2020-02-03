using System.Collections.Generic;

namespace PlayBoardGame.Models.ViewModels
{
    public class InvitationsViewModel
    {
        public class SentInvitationsViewModel
        {
            public List<InvitationsListViewModel> InvitedUsersList { get; set; }

            public string FiendEmailAddress { get; set; }
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