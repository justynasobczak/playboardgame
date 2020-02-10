using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IFriendInvitationRepository
    {
        IQueryable<AppUser> GetFriendsOfCurrentUser(string currentUserId);
        IQueryable<FriendInvitation> GetInvitationsReceivedByCurrentUser(string currentUserEmail);
        IQueryable<FriendInvitation> GetInvitationsSentByCurrentUser(string currentUserId);
        void AddInvitation(FriendInvitation invitation);
        void ChangeStatus(int invitationId, FriendInvitationStatus status);
        FriendInvitation GetInvitation(int invitationId);
    }
}