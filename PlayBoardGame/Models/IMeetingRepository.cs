using  System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        IQueryable<Meeting> Meetings { get; }

        IQueryable<Meeting> GetMeetingsForUser(string userId);
        
        void SaveMeeting(Meeting meeting);
    }
}