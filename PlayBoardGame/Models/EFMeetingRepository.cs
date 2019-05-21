using System.Linq;
using PlayBoardGame.Infrastructure;

namespace PlayBoardGame.Models
{
    public class EFMeetingRepository : IMeetingRepository
    {
        private readonly ApplicationDBContext _applicationDBContext;
        private readonly ContextProvider _contextProvider;

        public EFMeetingRepository(ApplicationDBContext applicationDBContext, ContextProvider contextProvider)
        {
            _applicationDBContext = applicationDBContext;
            _contextProvider = contextProvider;
        }

        public IQueryable<Meeting> Meetings => _applicationDBContext.Meetings;

        public IQueryable<Meeting> GetMeetingsOfCurrentUser()
        {
            var currentUser = _contextProvider.GetCurrentUserId().Result;
            var meetingsByOwner = _applicationDBContext.Meetings.Where(m => m.Organizer.Id == currentUser);
            var meetingsByInvitedUsers =
                _applicationDBContext.Meetings.Where(m => m.MeetingInvitedUser.Any(mu => mu.UserId == currentUser));
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
                }
            }
            _applicationDBContext.SaveChanges();

        }
    }
}