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

        public IQueryable<Game> GetGamesFromMeeting(int meetingId)
        {
            return _applicationDBContext.Games.Where(g => g.MeetingGame.Any(mg => mg.MeetingId == meetingId));
        }

        public Meeting GetMeeting(int meetingId)
        {
            return Meetings.FirstOrDefault(m => m.MeetingId == meetingId);
        }

        public string GetDescriptionFromMeeting(int meetingId)
        {
            return Meetings.Where(m => m.MeetingId == meetingId).Select(m => m.Description).FirstOrDefault();
        }

        public IQueryable<Meeting> GetMeetingsForUser(string userId)
        {
            return Meetings.Where(m => m.Organizer.Id == userId ||
                                       m.MeetingInvitedUser.Any(mu => mu.UserId == userId))
                .Distinct()
                .AsQueryable();
        }

        public void SaveMeeting(Meeting meeting)
        {
            if (meeting.MeetingId == 0)
            {
                _applicationDBContext.Meetings.Add(meeting);
            }
            else
            {
                var dbEntry = Meetings.FirstOrDefault(m => m.MeetingId == meeting.MeetingId);
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

        public void AddGameToMeeting(MeetingGame gameInMeeting)
        {
            _applicationDBContext.MeetingGame.Add(gameInMeeting);
            _applicationDBContext.SaveChanges();
        }

        public void SaveDescriptionForMeeting(string description, int meetingId)
        {
            var dbEntry = Meetings.FirstOrDefault(m => m.MeetingId == meetingId);
            if (dbEntry != null)
            {
                dbEntry.Description = description;
                _applicationDBContext.SaveChanges();
            }
        }

        // bozy: Refactor it to make 1-2 linq queries instead of playing with lists
        //DONE
        public MeetingGame RemoveGameFromMeeting(int gameId, int meetingId)
        {
            var dbEntry = _applicationDBContext.MeetingGame.FirstOrDefault(mg => mg.GameId == gameId
                                                                                 && mg.MeetingId == meetingId);
            if (dbEntry != null)
            {
                _applicationDBContext.MeetingGame.Remove(dbEntry);
                _applicationDBContext.SaveChanges();
            }

            return dbEntry;
        }

        public IQueryable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId)
        {
            return Meetings.Where(m => startDate <= m.EndDateTime && endDate >= m.StartDateTime &&
                                       (m.Organizer.Id == userId ||
                                        m.MeetingInvitedUser.Any(mu => mu.UserId == userId)))
                .Distinct()
                .AsQueryable();
        }

        public IQueryable<Meeting> GetOverlappingMeetingsForMeeting(DateTime startDate, DateTime endDate, int meetingId)
        {
            var checkedUsers = _applicationDBContext.Users
                .Where(u => u.OrganizedMeetings.Any(m => m.MeetingId == meetingId) ||
                            u.MeetingInvitedUser.Any(mu => mu.MeetingId == meetingId)).Distinct();

            var overlappingMeetings = new List<Meeting>();
            foreach (var user in checkedUsers)
            {
                var meetingsForUser = Meetings.Where(m =>
                    startDate <= m.EndDateTime && endDate >= m.StartDateTime &&
                    m.MeetingId != meetingId &&
                    (m.Organizer.Id == user.Id ||
                     m.MeetingInvitedUser.Any(mu => mu.UserId == user.Id)));

                foreach (var m in meetingsForUser)
                {
                    overlappingMeetings.Add(m);
                }
            }

            return overlappingMeetings.Distinct().AsQueryable();
        }
    }
}