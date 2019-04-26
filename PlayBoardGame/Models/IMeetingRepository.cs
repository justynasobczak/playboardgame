using  System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        IQueryable<Meeting> Meetings { get; }
        
        void SaveMeeting(Meeting meeting);
    }
}