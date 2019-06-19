using System;
using System.Collections.Generic;
using System.Linq;
using Remotion.Linq.Parsing.Structure.IntermediateModel;

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

        public Meeting GetMeeting(int meetingId)
        {
            return _applicationDBContext.Meetings.FirstOrDefault(m => m.MeetingID == meetingId);
        }

        public IQueryable<Meeting> GetMeetingsForUser(string userId)
        {
            var meetingsByOwner = _applicationDBContext.Meetings.Where(m => m.Organizer.Id == userId).ToList();
            var meetingsByInvitedUsers =
                _applicationDBContext.Meetings.Where(m => m.MeetingInvitedUser.Any(mu => mu.UserId == userId)).ToList();
            var myMeetings = meetingsByOwner.Union(meetingsByInvitedUsers);
            return myMeetings.AsQueryable();
        }

        public void SaveMeeting(Meeting meeting)
        {
            if (meeting.MeetingID == 0)
            {
                _applicationDBContext.Meetings.Add(meeting);
            }
            else
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

        public IQueryable<Meeting> GetOverlappingMeetings(IQueryable<Meeting> meetings, DateTime startDate,
            DateTime endDate)
        {
            var overlappingMeetings = new List<Meeting>();
            foreach (var meeting in meetings.ToList())
            {
                if (startDate <= meeting.EndDateTime && endDate >= meeting.StartDateTime)
                {
                    overlappingMeetings.Add(meeting);
                }
            }

            return overlappingMeetings.AsQueryable();
        }

        public IQueryable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId)
        {
            var meetings = GetMeetingsForUser(userId);
            return GetOverlappingMeetings(meetings, startDate, endDate);
        }

        public IQueryable<Meeting> GetOverlappingMeetingsForMeeting(DateTime startDate, DateTime endDate, int meetingId)
        {
            var meeting = GetMeeting(meetingId);
            var invitedUsers = _applicationDBContext.Users
                .Where(m => m.MeetingInvitedUser.Any(mu => mu.MeetingID == meetingId)).ToList();

            var meetingsForOrganizer = GetMeetingsForUser(meeting.OrganizerId).Where(m => m.MeetingID != meetingId);
            var meetingsForInvitedUsers = new List<Meeting>();
            foreach (var user in invitedUsers)
            {
                var meetingsForUser = GetMeetingsForUser(user.Id).ToList();
                foreach (var m in meetingsForUser)
                {
                    if (m.MeetingID != meetingId)
                    {
                        meetingsForInvitedUsers.Add(m);    
                    }
                }
            }
            var allMeetings = meetingsForOrganizer.Union(meetingsForInvitedUsers);
            return GetOverlappingMeetings(allMeetings, startDate, endDate);
        }
    }
}