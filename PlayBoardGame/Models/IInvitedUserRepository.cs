using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IInvitedUserRepository
    {
        IQueryable<AppUser> GetAvailableUsers(int meetingId);
        
        IQueryable<AppUser> GetInvitedUsers(int meetingId);

        Dictionary<string, bool> GetInvitedUsersList(int meetingId);

        void AddUserToMeeting(string UserId, int MeetingId, bool IsAccepted);

        MeetingInvitedUser RemoveUserFromMeeting(string UserId, int MeetingId);
    }
}