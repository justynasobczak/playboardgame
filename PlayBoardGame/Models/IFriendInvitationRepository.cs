using System.Linq;

namespace PlayBoardGame.Models
{
    public interface IFriendInvitationRepository
    {
        IQueryable<AppUser> GetFriends(string currentUserId);
    }
}