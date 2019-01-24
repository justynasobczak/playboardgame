using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using PlayBoardGame.Models;

namespace PlayBoardGame.Infrastructure
{
    public class ContextProvider
    {
        private IHttpContextAccessor _context;

        private UserManager<AppUser> _userManager;

        public ContextProvider(IHttpContextAccessor context, UserManager<AppUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public string GetCurrentUserName()
        {
            return _context.HttpContext.User?.Identity?.Name;
        }

        public async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(GetCurrentUserName());
            return user.Id.ToString();
        }

        public async Task<AppUser> GetCurrentUser()
        {
            return await _userManager.FindByNameAsync(GetCurrentUserName());
        }
    }
}
