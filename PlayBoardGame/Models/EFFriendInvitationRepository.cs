using System.Linq;

namespace PlayBoardGame.Models
{
    public class EFFriendInvitationRepository : IFriendInvitationRepository

    {
        private readonly ApplicationDBContext _applicationDbContext;

        public EFFriendInvitationRepository(ApplicationDBContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public IQueryable<AppUser> GetFriends(string currentUserId)
        {
            
            return _applicationDbContext.FriendInvitations
                .Where(fi =>
                    fi.Status == FriendInvitationStatus.Accepted &&
                    (fi.SenderId == currentUserId || fi.InvitedId == currentUserId))
                .Select(fi => fi.SenderId == currentUserId ? fi.Invited : fi.Sender);
        }

    }
}