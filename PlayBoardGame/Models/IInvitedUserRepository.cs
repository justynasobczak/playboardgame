using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IInvitedUserRepository
    {
        IQueryable<AppUser> GetAvailableUsers(int meetingId);
        
        IQueryable<AppUser> GetInvitedUsers(int meetingId);

        Dictionary<string, InvitationStatus> GetInvitedUsersList(int meetingId);

        void AddUserToMeeting(string userId, int meetingId, InvitationStatus status);

        MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId);

        void ChangeStatus(string userId, int meetingId, InvitationStatus status);
    }
}