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

        public MeetingController(IMeetingRepository meetingRepository, UserManager<AppUser> userManager,
            ILogger<MeetingController> logger)
        {
            _meetingRepository = meetingRepository;
            _userManager = userManager;
            _logger = logger;
        }

        public IActionResult List()
        {
            return View("Calendar");
        }

        public IActionResult Edit(int id)
        {
            var currentUserId = GetCurrentUserId().Result;
            var meeting = _meetingRepository.GetMeeting(id);
            if (meeting == null)
            {
                _logger.LogCritical($"{Constants.UnknownId} of meeting");
                return RedirectToAction(nameof(ErrorController.Error), "Error");
            }

            var timeZone = _userManager.FindByIdAsync(currentUserId).Result.TimeZone;

            var vm = new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizer = meeting.Organizer.FullName,
                Title = meeting.Title,
                MeetingId = meeting.MeetingId,
                StartDateTime = ToolsExtensions.ConvertToTimeZoneFromUtc(meeting.StartDateTime, timeZone, _logger)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                EndDateTime = ToolsExtensions.ConvertToTimeZoneFromUtc(meeting.EndDateTime, timeZone, _logger)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                Notes = meeting.Notes,
                Games = _meetingRepository.GetGamesForOrganizer(id, meeting.OrganizerId).OrderBy(g => g.Title),
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
            var timeZone = _userManager
                .FindByIdAsync(currentUserId).Result.TimeZone;
            if (ModelState.IsValid)
            {
                if (DateTime.TryParse(vm.StartDateTime, out var startDate) &&
                    DateTime.TryParse(vm.EndDateTime, out var endDate))
                {
                    startDateUTC = ToolsExtensions.ConvertFromTimeZoneToUtc(startDate, timeZone, _logger);
                    endDateUTC = ToolsExtensions.ConvertFromTimeZoneToUtc(endDate, timeZone, _logger);

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
                var selectedGames = vm.SelectedGames ?? new List<int>();
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

            vm.Games = _meetingRepository.GetGamesForOrganizer(vm.MeetingId, currentUserId).OrderBy(g => g.Title);
            return View(vm);
        }

        public IActionResult Create()
        {
            var currentUser = _userManager.FindByNameAsync(User.Identity.Name).Result;
            var timeZone = currentUser.TimeZone;

            return View(nameof(Edit), new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizer = currentUser.FullName,
                Games = _meetingRepository.GetGamesForOrganizer(0, currentUser.Id),
                StartDateTime = ToolsExtensions.ConvertToTimeZoneFromUtc(DateTime.UtcNow.AddHours(1), timeZone, _logger)
                    .ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture),
                EndDateTime = ToolsExtensions.ConvertToTimeZoneFromUtc(DateTime.UtcNow.AddHours(2), timeZone, _logger)
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

        private List<int> GetGameIdsFromMeeting(int meetingId)
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
                .ToList();
        }
    }
}