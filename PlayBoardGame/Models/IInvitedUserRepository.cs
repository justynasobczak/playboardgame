using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IInvitedUserRepository
    {
        IEnumerable<AppUser> GetAvailableUsers(int meetingId);

        IEnumerable<MeetingInvitedUser> GetInvitedUsersList(int meetingId);

        void AddUserToMeeting(string userId, int meetingId, InvitationStatus status);

        MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId);

        void ChangeStatus(string userId, int meetingId, InvitationStatus status);

        IQueryable<string> GetInvitedUsersEmails(int meetingId);
    }
}