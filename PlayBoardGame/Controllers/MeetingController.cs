using System;
using System.Collections.Generic;
using System.Globalization;
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
            var meeting = _meetingRepository.GetMeeting(id);
            if (meeting == null)
            {
                _logger.LogCritical($"{Constants.UnknownId} of meeting");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }

            var vm = new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizers = _userManager.Users.ToList(),
                OrganizerId = meeting.Organizer.Id,
                Title = meeting.Title,
                MeetingId = meeting.MeetingId,
                StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(meeting.StartDateTime, timeZone)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(meeting.EndDateTime, timeZone)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                Notes = meeting.Notes,
                Games = _gameRepository.Games.ToList(),
                SelectedGames = GetGameIdsFromMeeting(id),
                IsEditable = meeting.Organizer.UserName == User.Identity.Name,
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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MeetingViewModels.CreateEditMeetingViewModel vm)
        {
            var startDateUTC = new DateTime();
            var endDateUTC = new DateTime();
            var currentUserId = GetCurrentUserId().Result;
            if (ModelState.IsValid)
            {
                var timeZone = GetTimeZoneOfCurrentUser();

                if (DateTime.TryParse(vm.StartDateTime, out var startDate) &&
                    DateTime.TryParse(vm.EndDateTime, out var endDate))
                {
                    startDateUTC = TimeZoneInfo.ConvertTimeToUtc(startDate, timeZone);
                    endDateUTC = TimeZoneInfo.ConvertTimeToUtc(endDate, timeZone);
                    var overlappingMeetings = new List<string>();

                    overlappingMeetings = vm.MeetingId == 0
                        ? _meetingRepository.GetOverlappingMeetingsForUser(startDateUTC, endDateUTC, currentUserId)
                            .Select(m => m.Title).ToList()
                        : _meetingRepository.GetOverlappingMeetingsForMeeting(startDateUTC, endDateUTC, vm.MeetingId)
                            .Select(m => m.Title).ToList();

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
                        var overlappingMeetingsTitle = string.Join(", ", overlappingMeetings);

                        ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.Title),
                            $"{Constants.OverlappingMeetingsMessage}: {overlappingMeetingsTitle}");
                    }
                }
                else
                {
                    if (!DateTime.TryParse(vm.StartDateTime, out startDate))
                    {
                        ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.StartDateTime),
                            Constants.WrongDateTimeFormat);
                    }

                    if (!DateTime.TryParse(vm.EndDateTime, out endDate))
                    {
                        ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.EndDateTime),
                            Constants.WrongDateTimeFormat);
                    }
                }
            }

            if (ModelState.IsValid)
            {
                var organizer = await _userManager.FindByIdAsync(currentUserId);
                var meeting = new Meeting
                {
                    MeetingId = vm.MeetingId,
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
                var savedGames = GetGameIdsFromMeeting(meeting.MeetingId);
                var selectedGames = vm.SelectedGames ?? new int[0];
                var gamesToAdd = selectedGames.Except(savedGames).ToList();
                var gamesToRemove = savedGames.Except(selectedGames).ToList();

                if (gamesToAdd.Count > 0)
                {
                    foreach (var game in gamesToAdd)
                    {
                        var gameToAdd = new MeetingGame {GameId = game, MeetingId = meeting.MeetingId};
                        _meetingRepository.AddGameToMeeting(gameToAdd);
                    }
                }

                if (gamesToRemove.Count > 0)
                {
                    foreach (var game in gamesToRemove)
                    {
                        _meetingRepository.RemoveGameFromMeeting(game, meeting.MeetingId);
                    }
                }

                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction(nameof(Edit), new {id = meeting.MeetingId});
            }

            vm.Organizers = _userManager.Users.ToList();
            vm.Games = _gameRepository.Games.ToList();
            return View(vm);
        }

        public IActionResult Create()
        {
            var timeZone = GetTimeZoneOfCurrentUser();
            var currentUserId = GetCurrentUserId().Result;

            return View(nameof(Edit), new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizers = _userManager.Users.ToList(),
                OrganizerId = currentUserId,
                Games = _gameRepository.Games.ToList(),
                StartDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(1), timeZone)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                EndDateTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow.AddHours(2), timeZone)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                IsEditable = true,
                Address = new AddressViewModels()
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

        private int[] GetGameIdsFromMeeting(int meetingId)
        {
            // bozy use extension methods or linq or both
//            return _meetingRepository
//                .GetGamesFromMeeting(meetingId)
//                .Select(it => it.GameId)
//                .ToArray();
// OR
//            return (from it in _meetingRepository.GetGamesFromMeeting(meetingId) select it.GameId).ToArray();

            return _meetingRepository
                .GetGamesFromMeeting(meetingId)
                .Select(g => g.GameId)
                .ToArray();
        }
    }
}