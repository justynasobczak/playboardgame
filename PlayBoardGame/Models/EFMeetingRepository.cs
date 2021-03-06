using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

namespace PlayBoardGame.Models
{
    public class EFMeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;

        public EFMeetingRepository(ApplicationDBContext applicationDBContext)
        {
            _applicationDBContext = applicationDBContext;
        }

        public IEnumerable<Game> GetGamesFromMeeting(int meetingId)
        {
            return _applicationDBContext.Games.Where(g => g.MeetingGame.Any(mg => mg.MeetingId == meetingId));
        }

        public IQueryable<Game> GetGamesForOrganizer(int meetingId, string organizerId)
        {
            if (meetingId == 0)
            {
                return _applicationDBContext.Games.Where(g => g.GameAppUser.Any(gu => gu.AppUser.Id == organizerId))
                    .Select(g => new Game {Title = g.Title, GameId = g.GameId});
            }

            return _applicationDBContext.Games.Where(g => g.MeetingGame.Any(mg => mg.MeetingId == meetingId)
                                                          || g.GameAppUser.Any(gu => gu.UserId == organizerId))
                .Select(g => new Game {Title = g.Title, GameId = g.GameId});
        }

        public Meeting GetMeeting(int meetingId)
        {
            return _applicationDBContext.Meetings
                .Include(m => m.Organizer)
                .Include(m => m.MeetingGame)
                .ThenInclude(mg => mg.Game)
                .FirstOrDefault(m => m.MeetingId == meetingId);
        }

        public string GetDescriptionFromMeeting(int meetingId)
        {
            return _applicationDBContext.Meetings.Where(m => m.MeetingId == meetingId).Select(m => m.Description)
                .FirstOrDefault();
        }

        public IEnumerable<Meeting> GetMeetingsForUser(string userId)
        {
            return _applicationDBContext.Meetings.Where(m => m.Organizer.Id == userId ||
                                                             m.MeetingInvitedUser.Any(mu => mu.UserId == userId))
                .Distinct();
        }

        public IQueryable<Meeting> GetMeetingsForUserForNextDays(string userId, int days)
        {
            return _applicationDBContext.Meetings.Where(m => m.Organizer.Id == userId ||
                                                             m.MeetingInvitedUser.Any(mu => mu.UserId == userId))
                .Where(m => m.StartDateTime > DateTime.UtcNow && m.StartDateTime < DateTime.UtcNow.AddDays(days))
                .Distinct()
                .Include(m => m.Organizer)
                .Include(m => m.MeetingGame)
                .ThenInclude(mg => mg.Game)
                .Include(m => m.MeetingInvitedUser)
                .ThenInclude(iu => iu.AppUser);
        }

        public List<NotificationList> GetUsersToSendTomorrowsNotification()
        {
            var list = new List<NotificationList>();
            var meetings = GetTomorrowsMeetings().ToList();
            foreach (var meeting in meetings)
            {
                var users = _applicationDBContext.Users.Where(u =>
                    (u.MeetingInvitedUser.Any(mu => mu.MeetingId == meeting.MeetingId) ||
                     u.OrganizedMeetings.Any(m => m.MeetingId == meeting.MeetingId))).ToList();
                list.AddRange(from user in users
                    where !_applicationDBContext.TomorrowsMeetingsNotifications.Any(n =>
                        n.Participant.Id == user.Id && n.Meeting.MeetingId == meeting.MeetingId &&
                        (n.IfSent.Equals(true) || (n.IfSent.Equals(false) &&
                                                   n.NumberOfTries >= Constants.NumberOfTriesSendNotification)) &&
                        n.MeetingStartDateTime == meeting.StartDateTime)
                    select new NotificationList {Meeting = meeting, User = user});
            }

            return list;
        }

        public IQueryable<Meeting> GetTomorrowsMeetings()
        {
            return _applicationDBContext.Meetings
                .Where(m => m.StartDateTime > DateTime.UtcNow && m.StartDateTime < DateTime.UtcNow.AddDays(1))
                .Include(m => m.Organizer)
                .Include(m => m.MeetingGame)
                .ThenInclude(mg => mg.Game)
                .Include(m => m.MeetingInvitedUser)
                .ThenInclude(iu => iu.AppUser);
        }

        public void SaveMeeting(Meeting meeting)
        {
            if (meeting.MeetingId == 0)
            {
                _applicationDBContext.Meetings.Add(meeting);
            }
            else
            {
                var dbEntry = _applicationDBContext.Meetings.FirstOrDefault(m => m.MeetingId == meeting.MeetingId);
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
            var dbEntry = _applicationDBContext.Meetings.FirstOrDefault(m => m.MeetingId == meetingId);
            if (dbEntry == null) return;
            dbEntry.Description = description;
            _applicationDBContext.SaveChanges();
        }

        // bozy: Refactor it to make 1-2 linq queries instead of playing with lists
        //DONE
        public MeetingGame RemoveGameFromMeeting(int gameId, int meetingId)
        {
            var dbEntry = _applicationDBContext.MeetingGame.FirstOrDefault(mg => mg.GameId == gameId
                                                                                 && mg.MeetingId == meetingId);
            if (dbEntry == null) return dbEntry;
            _applicationDBContext.MeetingGame.Remove(dbEntry);
            _applicationDBContext.SaveChanges();

            return dbEntry;
        }

        public IEnumerable<Meeting> GetOverlappingMeetingsForUser(DateTime startDate, DateTime endDate, string userId)
        {
            return _applicationDBContext.Meetings.Where(m => startDate <= m.EndDateTime && endDate >= m.StartDateTime &&
                                                             (m.Organizer.Id == userId ||
                                                              m.MeetingInvitedUser.Any(mu => mu.UserId == userId)))
                .Distinct();
        }

        public IEnumerable<Meeting> GetOverlappingMeetingsForMeeting(DateTime startDate, DateTime endDate,
            int meetingId)
        {
            var checkedUsers = _applicationDBContext.Users
                .Where(u => u.OrganizedMeetings.Any(m => m.MeetingId == meetingId) ||
                            u.MeetingInvitedUser.Any(mu => mu.MeetingId == meetingId)).Distinct().ToList();

            var overlappingMeetings = new List<Meeting>();
            foreach (var meetingsForUser in checkedUsers.Select(user => _applicationDBContext.Meetings.Where(m =>
                startDate <= m.EndDateTime && endDate >= m.StartDateTime &&
                m.MeetingId != meetingId &&
                (m.Organizer.Id == user.Id ||
                 m.MeetingInvitedUser.Any(mu => mu.UserId == user.Id))).ToList()))
            {
                overlappingMeetings.AddRange(meetingsForUser);
            }

            return overlappingMeetings.Distinct();
        }
    }
}