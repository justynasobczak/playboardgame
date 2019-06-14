using System;
using System.Collections.Generic;
using  System.Linq;

namespace PlayBoardGame.Models
{
    public interface IMeetingRepository
    {
        IQueryable<Meeting> Meetings { get; }

        IQueryable<Meeting> GetMeetingsForUser(string userId);
        
        void SaveMeeting(Meeting meeting);

        IQueryable<Meeting> GetOverlappingMeetings(IQueryable<Meeting> meetings, DateTime startDate, DateTime endDate);

        IQueryable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId);
    }
}