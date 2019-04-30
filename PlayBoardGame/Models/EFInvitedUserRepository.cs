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
            //var meeting = _applicationDbContext.Meetings.FirstOrDefault(m => m.MeetingID == meetingId);
            var invitedUsers = _userManager.Users.Where(u => u.MeetingInvitedUser.Any(mu => mu.MeetingID == meetingId));
            return invitedUsers;
        }
        
        public IQueryable<AppUser> GetAvailableUsers(int meetingId)
        {
            var invitedUsers = GetInvitedUsers(meetingId);
            var availableUsers = _applicationDbContext.Users.Except(invitedUsers);
            return availableUsers;
        }

        public void AddUserToMeeting(string userId, int meetingId)
        {
            var meeting = _applicationDbContext.Meetings.FirstOrDefault(m => m.MeetingID == meetingId);

            var user = _userManager.FindByIdAsync(userId).Result;

            _applicationDbContext.Set<MeetingInvitedUser>().Add(new MeetingInvitedUser
            {
                MeetingID = meeting.MeetingID,
                UserId = user.Id
            });

            _applicationDbContext.SaveChanges();
        }

        public MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId)
        {
            var meeting = _applicationDbContext.Meetings.FirstOrDefault(m => m.MeetingID == meetingId);

            var user = _userManager.FindByIdAsync(userId).Result;

            var dbEntry = _applicationDbContext.MeetingInvitedUser.FirstOrDefault
                (mu => mu.MeetingID == meeting.MeetingID && mu.UserId == user.Id);
            if (dbEntry != null)
            {
                _applicationDbContext.Set<MeetingInvitedUser>().Remove(dbEntry);

                _applicationDbContext.SaveChanges();
            }

            return dbEntry;
        }
    }
}