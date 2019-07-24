using System;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        IQueryable<Meeting> Meetings { get; }
        
        Meeting GetMeeting(int meetingId);

        IQueryable<Meeting> GetMeetingsForUser(string userId);
        
        void SaveMeeting(Meeting meeting);
        
        void AddGameToMeeting(MeetingGame gameInMeeting);

        MeetingGame RemoveGameFromMeeting(int gameId, int meetingId);

        IQueryable<Game> GetGamesFromMeeting(int meetingId);

        IQueryable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId);

        IQueryable<Meeting> GetOverlappingMeetingsForMeeting(DateTime startDate, DateTime endDate, int meetingId);
    }
}