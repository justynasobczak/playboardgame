using System.Collections.Generic;
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

        public Dictionary<string, InvitationStatus> GetInvitedUsersList(int meetingId)
        {
            var invitedUsersList = new Dictionary<string, InvitationStatus>();
            var entry = new List<MeetingInvitedUser>();
            entry = _applicationDbContext.MeetingInvitedUser.Where(mu => mu.MeetingID == meetingId).ToList();

            foreach (var item in entry)
            {
                invitedUsersList.Add(item.UserId, item.Status);
            }

            return invitedUsersList;
        }

        public IQueryable<AppUser> GetAvailableUsers(int meetingId)
        {
            var invitedUsers = GetInvitedUsers(meetingId);
            var availableUsers = _applicationDbContext.Users.Except(invitedUsers);
            return availableUsers;
        }

        public void AddUserToMeeting(string userId, int meetingId, InvitationStatus status)
        {
            _applicationDbContext.Set<MeetingInvitedUser>().Add(new MeetingInvitedUser
            {
                MeetingID = meetingId,
                UserId = userId,
                Status = status
            });

            _applicationDbContext.SaveChanges();
        }

        public MeetingInvitedUser RemoveUserFromMeeting(string userId, int meetingId)
        {
            var dbEntry = _applicationDbContext.MeetingInvitedUser.FirstOrDefault
                (mu => mu.MeetingID == meetingId && mu.UserId == userId);
            _applicationDbContext.Set<MeetingInvitedUser>().Remove(dbEntry);
            _applicationDbContext.SaveChanges();

            return dbEntry;
        }

        public void ChangeStatus(string userId, int meetingId, InvitationStatus status)
        {
            var dbEntry = _applicationDbContext.MeetingInvitedUser.FirstOrDefault
                (mu => mu.MeetingID == meetingId && mu.UserId == userId);
            if (dbEntry == null) return;
            dbEntry.Status = status;
                
            _applicationDbContext.Set<MeetingInvitedUser>().Update(dbEntry);
            _applicationDbContext.SaveChanges();
        }
    }
}