using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class EFInvitedUserRepository : IInvitedUserRepository
    {
        private readonly ApplicationDBContext _applicationDbContext;
        private readonly UserManager<AppUser> _userManager;

        public EFInvitedUserRepository(ApplicationDBContext applicationDbContext,
            UserManager<AppUser> userManager)
        {
            _applicationDbContext = applicationDbContext;
            _userManager = userManager;
        }

        public IQueryable<AppUser> GetInvitedUsers(int meetingId)
        {
            return _userManager.Users.Where(u => u.MeetingInvitedUser.Any(mu => mu.MeetingId == meetingId));
        }
        
        public IQueryable<MeetingInvitedUser> GetInvitedUsersList(int meetingId)
        {
            return _applicationDbContext.MeetingInvitedUser.Where(mu => mu.MeetingId == meetingId)
                .Include(mu => mu.AppUser)
                .Include(mu => mu.Meeting);
        }

        public IQueryable<AppUser> GetAvailableUsers(int meetingId)
        {
            var invitedUsers = GetInvitedUsers(meetingId);
            var availableUsers = _applicationDbContext.Users.Except(invitedUsers);
            return availableUsers;
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
            if (dbEntry != null)
            {
                _applicationDbContext.MeetingInvitedUser.Remove(dbEntry);
                _applicationDbContext.SaveChanges();
            }

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
    }
}