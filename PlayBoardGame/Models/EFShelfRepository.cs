using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using PlayBoardGame.Infrastructure;

namespace PlayBoardGame.Models
{
    public class EFShelfRepository : IShelfRepository
    {
        private ApplicationDBContext _applicationDBContext;
        private UserManager<AppUser> _userManager;
        private ContextProvider _contextProvider;


        public EFShelfRepository(ApplicationDBContext applicationDBContext, UserManager<AppUser> userManager, ContextProvider contextProvider)
        {
            _applicationDBContext = applicationDBContext;
            _userManager = userManager;
            _contextProvider = contextProvider;
        }

        public IQueryable<Game> Shelf =>
            _applicationDBContext.Games.Where(g => g.GameAppUser.Any(gu => gu.AppUser.Id == GetCurrentUserId().Result));

        public IQueryable<Game> AvailableGames => _applicationDBContext.Games.Except(Shelf);

        public void SaveShelf(int GameId)
        {
            Game game = _applicationDBContext.Games.FirstOrDefault(g => g.GameID == GameId);

            //AppUser user = await _userManager.FindByNameAsync(_contextProvider.GetCurrentUserName());
            AppUser user = GetCurrentUser().Result;

            GameAppUser gameUser = new GameAppUser();

            gameUser.Game = game;
            gameUser.AppUser = user;
            gameUser.GameID = game.GameID;
            gameUser.UserId = user.Id;

            user.GameAppUser.Add(gameUser);
            //game.GameAppUser.Add(gameUser);

            //_applicationDBContext.GameAppUser.Add(gameUser);
            _applicationDBContext.SaveChanges();
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(_contextProvider.GetCurrentUserName());
            return user.Id.ToString();
        }

        private async Task<AppUser> GetCurrentUser()
        {
            return await _userManager.FindByNameAsync(_contextProvider.GetCurrentUserName());
        }
    }
}
