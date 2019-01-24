using Microsoft.AspNetCore.Http;

namespace PlayBoardGame.Infrastructure
{
    public class ContextProvider
    {
        private IHttpContextAccessor _context;

        public ContextProvider(IHttpContextAccessor context)
        {
            _context = context;
        }

        public string GetCurrentUserName()
        {
            return _context.HttpContext.User?.Identity?.Name;
        }
    }
}
