using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IInvitedUserRepository
    {
        IQueryable<AppUser> GetAvailableUsers(int meetingId);
        
        IQueryable<AppUser> GetInvitedUsers(int meetingId);

        void AddUserToMeeting(string UserId, int MeetingId);

        MeetingInvitedUser RemoveUserFromMeeting(string UserId, int MeetingId);
    }
}