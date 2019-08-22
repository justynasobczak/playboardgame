using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        Meeting GetMeeting(int meetingId);

        string GetDescriptionFromMeeting(int meetingId);

        IEnumerable<Meeting> GetMeetingsForUser(string userId);

        void SaveMeeting(Meeting meeting);

        void AddGameToMeeting(MeetingGame gameInMeeting);

        void SaveDescriptionForMeeting(string description, int meetingId);

        MeetingGame RemoveGameFromMeeting(int gameId, int meetingId);

        IEnumerable<Game> GetGamesFromMeeting(int meetingId);

        IEnumerable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId);

        IEnumerable<Meeting> GetOverlappingMeetingsForMeeting(DateTime startDate, DateTime endDate, int meetingId);
    }
}