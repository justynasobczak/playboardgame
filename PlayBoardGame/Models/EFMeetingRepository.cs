using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFMeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFMeetingRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public IQueryable<Meeting> Meetings => _applicationDBContext.Meetings;

        public IQueryable<Meeting> GetMeetingsForUser(string userId)
        {
            var meetingsByOwner = _applicationDBContext.Meetings.Where(m => m.Organizer.Id == userId);
            var meetingsByInvitedUsers =
                _applicationDBContext.Meetings.Where(m => m.MeetingInvitedUser.Any(mu => mu.UserId == userId));
            var myMeetings = meetingsByOwner.Union(meetingsByInvitedUsers);
            return myMeetings;
        }
        
        public void SaveMeeting(Meeting meeting)
        {
            if (meeting.MeetingID == 0)
            {
                _applicationDBContext.Meetings.Add(meeting);
            } else
            {
                var dbEntry = _applicationDBContext.Meetings.FirstOrDefault(m => m.MeetingID == meeting.MeetingID);
                if (dbEntry != null)
                {
                    dbEntry.Title = meeting.Title;
                    dbEntry.StartDateTime = meeting.StartDateTime;
                    dbEntry.EndDateTime = meeting.EndDateTime;
                    dbEntry.Organizer = meeting.Organizer;
                    dbEntry.City = meeting.City;
                    dbEntry.Street = meeting.Street;
                    dbEntry.PostalCode = meeting.PostalCode;
                    dbEntry.Country = meeting.Country;
                    dbEntry.Notes = meeting.Notes;
                }
            }
            _applicationDBContext.SaveChanges();

        }
        
        public List<Meeting> GetConflictedMeetings(List<Meeting> meetings, DateTime startDate, DateTime endDate)
        {
            var conflictedMeetings = new List<Meeting>();
            foreach (var meeting in meetings)
            {
                if (startDate <= meeting.EndDateTime && endDate >= meeting.StartDateTime)
                {
                    conflictedMeetings.Add(meeting);
                }
            }
            return conflictedMeetings;
        }
    }
}