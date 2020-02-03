using System;

namespace PlayBoardGame.Models
{
    public class FriendInvitation
    {
        public int FriendInvitationId { get; set; }
        
        public DateTime PostDateTime { get; set; }
        
        public string SenderId { get; set; }
        public AppUser Sender { get; set; }
        
        public string InvitedId { get; set; }
        public AppUser Invited { get; set; }
        
        public string InvitedEmail { get; set; }
        
        public FriendInvitationStatus Status { get; set; }
    }
    
    public enum FriendInvitationStatus
    {
        Pending,
        Accepted,
        Rejected,
        Deleted
    }
}