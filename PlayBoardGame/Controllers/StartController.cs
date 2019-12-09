using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Infrastructure;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class StartController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMeetingRepository _meetingRepository;
        private readonly ILogger<StartController> _logger;

        public StartController(UserManager<AppUser> userManager, IMeetingRepository meetingRepository,
            ILogger<StartController> logger)
        {
            _userManager = userManager;
            _meetingRepository = meetingRepository;
            _logger = logger;
        }

        public ViewResult Index()
        {
            var currentUserId = GetCurrentUserId().Result;
            var upcomingMeetings = _meetingRepository.GetMeetingsForUserForNextDays(currentUserId, 7);
            var list = new List<UpcomingMeetings>();
            var timeZone = _userManager
                .FindByIdAsync(currentUserId).Result.TimeZone;
            foreach (var meeting in upcomingMeetings)
            {
                var games = string.Join(", ", meeting.MeetingGame.Select(mg => mg.Game.Title));
                var people = string.Join(", ", meeting.MeetingInvitedUser.Select(iu => iu.AppUser.FullName));
                var startDateUtc = ToolsExtensions.ConvertFromTimeZoneToUtc(meeting.StartDateTime, timeZone, _logger);
                list.Add(new UpcomingMeetings()
                    {StartDate = startDateUtc.ToString(), Games = games, People = people});
            }

            var mv = new StartViewModels {UpcomingMeetings = list};
            return View(mv);
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}