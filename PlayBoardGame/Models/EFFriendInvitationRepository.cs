using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace PlayBoardGame.Models
{
    public class EFFriendInvitationRepository : IFriendInvitationRepository

    {
        private readonly ApplicationDBContext _applicationDbContext;

        public EFFriendInvitationRepository(ApplicationDBContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public IQueryable<AppUser> GetFriendsOfCurrentUser(string currentUserId)
        {
            return _applicationDbContext.FriendInvitations
                .Where(fi =>
                    fi.Status == FriendInvitationStatus.Accepted &&
                    (fi.SenderId == currentUserId || fi.InvitedId == currentUserId))
                .Select(fi => fi.SenderId == currentUserId ? fi.Invited : fi.Sender);
        }

        public IQueryable<FriendInvitation> GetInvitationsSentByCurrentUser(string currentUserId)
        {
            return _applicationDbContext.FriendInvitations
                .Where(fi => fi.SenderId == currentUserId);
        }

        public void AddInvitation(FriendInvitation invitation)
        {
            invitation.PostDateTime = DateTime.UtcNow;
            invitation.Status = FriendInvitationStatus.Pending;
            _applicationDbContext.FriendInvitations.Add(invitation);

            _applicationDbContext.SaveChanges();
        }

        public void ChangeStatus(int invitationId, FriendInvitationStatus status)
        {
            var dbEntry = _applicationDbContext.FriendInvitations.FirstOrDefault
                (fi => fi.FriendInvitationId == invitationId);
            if (dbEntry == null) return;
            dbEntry.Status = status;

            _applicationDbContext.FriendInvitations.Update(dbEntry);
            _applicationDbContext.SaveChanges();
        }

        public FriendInvitation GetInvitation(int invitationId)
        {
            return _applicationDbContext.FriendInvitations.Include(fi => fi.Invited)
                .Include(fi => fi.Sender)
                .FirstOrDefault(fi => fi.FriendInvitationId == invitationId);
        }
    }
}