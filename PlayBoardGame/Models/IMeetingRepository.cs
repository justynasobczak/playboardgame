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

        List<Meeting> GetConflictedMeetings(List<Meeting> meetings, DateTime startDate, DateTime endDate);
    }
}