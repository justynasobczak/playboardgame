using  System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        IQueryable<Meeting> Meetings { get; }

        IQueryable<Meeting> GetMeetingsOfCurrentUser();
        
        void SaveMeeting(Meeting meeting);
    }
}