using System.Linq;
using Microsoft.AspNetCore.Identity;

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
            var invitedUsers = _userManager.Users.Where(u => u.MeetingInvitedUser.Any(mu => mu.MeetingID == meetingId));
            return invitedUsers;
        }

        public IQueryable<AppUser> GetAvailableUsers(int meetingId)
        {
            var invitedUsers = GetInvitedUsers(meetingId);
            var availableUsers = _applicationDbContext.Users.Except(invitedUsers);
            return availableUsers;
        }

        public void AddUserToMeeting(string userId, int meetingId, bool IsAccepted)
        {
            _applicationDbContext.Set<MeetingInvitedUser>().Add(new MeetingInvitedUser
            {
                MeetingID = meetingId,
                UserId = userId,
                IsAccepted = IsAccepted
            });

            _applicationDbContext.SaveChanges();
        }

        public MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId)
        {
            var dbEntry = _applicationDbContext.MeetingInvitedUser.FirstOrDefault
                (mu => mu.MeetingID == meetingId && mu.UserId == userId);
            if (dbEntry != null)
            {
                _applicationDbContext.Set<MeetingInvitedUser>().Remove(dbEntry);

                _applicationDbContext.SaveChanges();
            }
            return dbEntry;
        }
    }
}