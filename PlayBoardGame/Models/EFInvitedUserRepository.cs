using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class EFInvitedUserRepository : IInvitedUserRepository
    {
        private readonly ApplicationDBContext _applicationDbContext;

        public EFInvitedUserRepository(ApplicationDBContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IEnumerable<MeetingInvitedUser> GetInvitedUsersList(int meetingId)
        {
            return _applicationDbContext.MeetingInvitedUser.Where(mu => mu.MeetingId == meetingId)
                .Include(mu => mu.AppUser)
                .Include(mu => mu.Meeting);
        }

        public IEnumerable<AppUser> GetAvailableUsers(int meetingId)
        {
            return _applicationDbContext.Users.Where(u => u.MeetingInvitedUser.All(mu => mu.MeetingId != meetingId))
                .Where(u => u.OrganizedMeetings.All(m => m.MeetingId != meetingId));
        }

        public void AddUserToMeeting(string userId, int meetingId, InvitationStatus status)
        {
            _applicationDbContext.MeetingInvitedUser.Add(new MeetingInvitedUser
            {
                MeetingId = meetingId,
                UserId = userId,
                Status = status
            });

            _applicationDbContext.SaveChanges();
        }

        public MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId)
        {
            var dbEntry = _applicationDbContext.MeetingInvitedUser.FirstOrDefault
                (mu => mu.MeetingId == meetingId && mu.UserId == userId);
            if (dbEntry == null) return dbEntry;
            _applicationDbContext.MeetingInvitedUser.Remove(dbEntry);
            _applicationDbContext.SaveChanges();

            return dbEntry;
        }

        public void ChangeStatus(string userId, int meetingId, InvitationStatus status)
        {
            var dbEntry = _applicationDbContext.MeetingInvitedUser.FirstOrDefault
                (mu => mu.MeetingId == meetingId && mu.UserId == userId);
            if (dbEntry == null) return;
            dbEntry.Status = status;

            _applicationDbContext.MeetingInvitedUser.Update(dbEntry);
            _applicationDbContext.SaveChanges();
        }

        public IQueryable<string> GetInvitedUsersEmails(int meetingId)
        {
            return _applicationDbContext.Users.Where(u => u.MeetingInvitedUser.Any(iu => iu.MeetingId == meetingId))
                .Select(u => u.Email);
        }
    }
}