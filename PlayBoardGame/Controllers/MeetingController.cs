using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PlayBoardGame.Models.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PlayBoardGame.Infrastructure;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<MeetingController> _logger;
        private readonly IGameRepository _gameRepository;

        public MeetingController(IMeetingRepository meetingRepository, UserManager<AppUser> userManager,
            ILogger<MeetingController> logger, IGameRepository gameRepository)
        {
            _meetingRepository = meetingRepository;
            _userManager = userManager;
            _logger = logger;
            _gameRepository = gameRepository;
        }

        public IActionResult List()
        {
            return View("Calendar");
        }

        public IActionResult Edit(int id)
        {
            var timeZone = GetTimeZoneOfCurrentUser();
            var currentUserId = GetCurrentUserId().Result;
            var meeting = _meetingRepository.GetMeeting(id);
            if (meeting == null) return RedirectToAction("Error", "Error");
            var vm = new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizers = _userManager.Users.ToList(),
                OrganizerId = meeting.Organizer.Id,
                Title = meeting.Title,
                MeetingID = meeting.MeetingID,
                StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartDateTime, timeZone),
                EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndDateTime, timeZone),
                Notes = meeting.Notes,
                Games = _gameRepository.Games.ToList(),
                SelectedGames = GetSelectedGames(id),
                IsEditable = meeting.OrganizerId == currentUserId,
                Address = new AddressViewModels
                {
                    Street = meeting.Street,
                    City = meeting.City,
                    PostalCode = meeting.PostalCode,
                    Country = meeting.Country
                }
            };
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(MeetingViewModels.CreateEditMeetingViewModel vm)
        {
            var timeZone = GetTimeZoneOfCurrentUser();
            var startDateUTC = TimeZoneInfo.ConvertTimeToUtc(vm.StartDateTime, timeZone);
            var endDateUTC = TimeZoneInfo.ConvertTimeToUtc(vm.EndDateTime, timeZone);
            var currentUserId = GetCurrentUserId().Result;
            var overlappingMeetings = new List<Meeting>();
            
            overlappingMeetings = vm.MeetingID == 0 ? _meetingRepository.GetOverlappingMeetingsForUser(startDateUTC, endDateUTC, currentUserId).ToList() 
                : _meetingRepository.GetOverlappingMeetingsForMeeting(startDateUTC, endDateUTC, vm.MeetingID).ToList();

            if (!ToolsExtensions.IsDateInFuture(startDateUTC))
            {
                ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.StartDateTime),
                    Constants.FutureDateInPastMessage);
            }

            if (!ToolsExtensions.IsStartDateBeforeEndDate(startDateUTC, endDateUTC))
            {
                ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.EndDateTime),
                    Constants.EndDateBeforeStartMessage);
            }

            if (overlappingMeetings.Count > 0)
            {
                var overlappingMeetingsTitle = ": ";
                foreach (var meeting in overlappingMeetings)
                {
                    overlappingMeetingsTitle += meeting.Title + " ";
                }
                
                ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.OrganizerId),
                    Constants.OverlappingMeetingsMessage + overlappingMeetingsTitle);
            }

            if (ModelState.IsValid)
            {
                var organizer = await _userManager.FindByIdAsync(currentUserId);
                var meeting = new Meeting
                {
                    MeetingID = vm.MeetingID,
                    Title = vm.Title,
                    StartDateTime = startDateUTC,
                    EndDateTime = endDateUTC,
                    Organizer = organizer,
                    Street = vm.Address.Street,
                    PostalCode = vm.Address.PostalCode,
                    City = vm.Address.City,
                    Country = vm.Address.Country,
                    Notes = vm.Notes
                };
                _meetingRepository.SaveMeeting(meeting);
                if (vm.SelectedGames?.Count() > 0)
                {
                    foreach (var game in vm.SelectedGames)
                    {
                        var selectedGame = new MeetingGame {GameID = game, MeetingID = vm.MeetingID};
                        _meetingRepository.AddGameToMeeting(selectedGame);
                    }                  
                }
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction("Edit", new {id = meeting.MeetingID});
            }

            vm.Organizers = _userManager.Users.ToList();
            vm.Games = _gameRepository.Games.ToList();
            return View(vm);
        }

        public IActionResult Create()
        {
            var timeZone = GetTimeZoneOfCurrentUser();
            var currentUserId = GetCurrentUserId().Result;
            return View("Edit", new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizers = _userManager.Users.ToList(),
                OrganizerId = currentUserId,
                Games = _gameRepository.Games.ToList(),
                StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(1), timeZone),
                EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(2), timeZone),
                IsEditable = true
            });
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }

        private TimeZoneInfo GetTimeZoneOfCurrentUser()
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var currentUserTimeZone = currentUser.TimeZone;
            return ToolsExtensions.ConvertTimeZone(currentUserTimeZone, _logger);
        }

        private int[] GetSelectedGames(int meetingId)
        {
            var games = _meetingRepository.GetGamesFromMeeting(meetingId).ToList();
            var list = new List<int>();
            foreach (var game in games)
            {
                list.Add(game.GameID);
            }

            return list.ToArray();
        }
    }
}