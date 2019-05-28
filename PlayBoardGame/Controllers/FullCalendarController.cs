using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class FullCalendarController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly UserManager<AppUser> _userManager;
 
        public FullCalendarController(IMeetingRepository meetingRepository, UserManager<AppUser> userManager)
        {
            _meetingRepository = meetingRepository;
            _userManager = userManager;
        }

        [HttpGet]
        public IEnumerable<Meeting> Get()
        {
            var currentUser = GetCurrentUserId().Result;
            return _meetingRepository.GetMeetingsForUser(currentUser);
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}