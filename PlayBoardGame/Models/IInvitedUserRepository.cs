using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IInvitedUserRepository
    {
        IQueryable<AppUser> GetAvailableUsers(int meetingId);
        
        IQueryable<AppUser> GetInvitedUsers(int meetingId);

        Dictionary<string, bool> GetInvitedUsersList(int meetingId);

        void AddUserToMeeting(string userId, int meetingId, bool isAccepted);

        MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId);

        void ChangeIsAccepted(string userId, int meetingId);
    }
}