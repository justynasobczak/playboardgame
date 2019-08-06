using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class HomeAddressController : Controller
    {
        private readonly UserManager<AppUser> _userManager;

        public HomeAddressController(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public Dictionary<string, string> Get()
        {
            var currentUser = GetCurrentUser().Result;
            var address = new Dictionary<string, string>
            {
                {"Street", currentUser.Street},
                {"City", currentUser.City},
                {"PostalCode", currentUser.PostalCode},
                {"Country", currentUser.Country}
            };

            return address;
        }

        private async Task<AppUser> GetCurrentUser()
        {
            return await _userManager.FindByNameAsync(User.Identity.Name);
        }
    }
}