using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IFriendInvitationRepository
    {
        IEnumerable<AppUser> GetFriendsOfCurrentUser(string currentUserId);
        IQueryable<FriendInvitation> GetInvitationsReceivedByCurrentUser(string currentUserEmail);
        IQueryable<FriendInvitation> GetInvitationsSentByCurrentUser(string currentUserId);
        void AddInvitation(FriendInvitation invitation);
        void ChangeStatus(int invitationId, FriendInvitationStatus status, AppUser currentUser);
        FriendInvitation GetInvitation(int invitationId);
        bool IfInvitationWasSentByCurrentUser(string currentUserId, string InvitedEmail);
        public bool IfInvitationWasReceivedByCurrentUser(string senderId, string currentUserEmail);
    }
}