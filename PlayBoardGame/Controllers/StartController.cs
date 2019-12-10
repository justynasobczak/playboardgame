using System.Collections.Generic;
using System.Globalization;
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
            var timeZone = _userManager
                .FindByIdAsync(currentUserId).Result.TimeZone;
            var upcomingMeetings = _meetingRepository.GetMeetingsForUserForNextDays(currentUserId, 7)
                .OrderBy(m => m.StartDateTime);
            var list = (from meeting in upcomingMeetings
                let games = string.Join(", ", meeting.MeetingGame.Select(mg => mg.Game.Title))
                let people = string.Join(", ", meeting.MeetingInvitedUser.Select(iu => iu.AppUser.FullName))
                    .Insert(0, $"{meeting.Organizer.FullName}, ")
                let startDateUtc = ToolsExtensions.ConvertToTimeZoneFromUtc(meeting.StartDateTime, timeZone, _logger)
                select new UpcomingMeetings()
                {
                    StartDate = startDateUtc.ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                    Games = games, People = people,
                    Url = Url.Action("Edit", "Meeting", new {id = meeting.MeetingId.ToString()},
                        HttpContext.Request.Scheme)
                }).ToList();

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