using System;
using System.Collections.Generic;
using  System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        IQueryable<Meeting> Meetings { get; }
        
        Meeting GetMeeting(int meetingId);

        IQueryable<Meeting> GetMeetingsForUser(string userId);
        
        void SaveMeeting(Meeting meeting);

        IQueryable<Meeting> GetOverlappingMeetings(IQueryable<Meeting> meetings, DateTime startDate, DateTime endDate);

        IQueryable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId);

        IQueryable<Meeting> GetOverlappingMeetingsForMeeting(DateTime startDate, DateTime endDate, int meetingId);
    }
}