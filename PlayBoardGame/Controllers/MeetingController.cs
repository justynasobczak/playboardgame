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
using PlayBoardGame.Email.SendGrid;
using PlayBoardGame.Email.Template;
using PlayBoardGame.Infrastructure;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger<MeetingController> _logger;
        private readonly IEmailTemplateSender _templateSender;
        private readonly IInvitedUserRepository _invitedUserRepository;

        public MeetingController(IMeetingRepository meetingRepository, UserManager<AppUser> userManager,
            ILogger<MeetingController> logger, IEmailTemplateSender templateSender,
            IInvitedUserRepository invitedUserRepository)
        {
            _meetingRepository = meetingRepository;
            _userManager = userManager;
            _logger = logger;
            _templateSender = templateSender;
            _invitedUserRepository = invitedUserRepository;
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
            var startDateUtc = new DateTime();
            var endDateUtc = new DateTime();
            var currentUserId = GetCurrentUserId().Result;
            var timeZone = _userManager
                .FindByIdAsync(currentUserId).Result.TimeZone;
            if (ModelState.IsValid)
            {
                if (DateTime.TryParse(vm.StartDateTime, out var startDate) &&
                    DateTime.TryParse(vm.EndDateTime, out var endDate))
                {
                    startDateUtc = ToolsExtensions.ConvertFromTimeZoneToUtc(startDate, timeZone, _logger);
                    endDateUtc = ToolsExtensions.ConvertFromTimeZoneToUtc(endDate, timeZone, _logger);

                    var overlappingMeetings = new List<string>();

                    overlappingMeetings = vm.MeetingId == 0
                        ? _meetingRepository.GetOverlappingMeetingsForUser(startDateUtc, endDateUtc, currentUserId)
                            .Select(m => m.Title).ToList()
                        : _meetingRepository.GetOverlappingMeetingsForMeeting(startDateUtc, endDateUtc, vm.MeetingId)
                            .Select(m => m.Title).ToList();

                    if (!ToolsExtensions.IsDateInFuture(startDateUtc))
                    {
                        ModelState.AddModelError(nameof(MeetingViewModels.CreateEditMeetingViewModel.StartDateTime),
                            Constants.FutureDateInPastMessage);
                    }

                    if (!ToolsExtensions.IsStartDateBeforeEndDate(startDateUtc, endDateUtc))
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
                    StartDateTime = startDateUtc,
                    EndDateTime = endDateUtc,
                    Organizer = organizer,
                    Street = vm.Address.Street,
                    PostalCode = vm.Address.PostalCode,
                    City = vm.Address.City,
                    Country = vm.Address.Country,
                    Notes = vm.Notes
                };
                var baseMeeting = _meetingRepository.GetMeeting(meeting.MeetingId);
                string changes = null;
                if (baseMeeting != null)
                {
                    changes = CompareMeetingData(baseMeeting, meeting, timeZone);
                }

                _meetingRepository.SaveMeeting(meeting);
                var savedGames = GetGameIdsFromMeeting(meeting.MeetingId);
                var selectedGames = vm.SelectedGames ?? new List<int>();
                var gamesToAdd = selectedGames.Except(savedGames).ToList();
                var gamesToRemove = savedGames.Except(selectedGames).ToList();

                if (gamesToAdd.Count > 0)
                {
                    foreach (var gameToAdd in gamesToAdd.Select(game => new MeetingGame
                        {GameId = game, MeetingId = meeting.MeetingId}))
                    {
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
                if (changes == null) return RedirectToAction(nameof(Edit), new {id = meeting.MeetingId});
                var appLink = Url.Action(nameof(Edit), "Meeting", new {id = baseMeeting.MeetingId},
                    HttpContext.Request.Scheme);
                foreach (var email in _invitedUserRepository.GetInvitedUsersEmails(baseMeeting.MeetingId))
                {
                    _templateSender.SendGeneralEmailAsync(new SendEmailDetails
                            {
                                IsHTML = true,
                                ToEmail = email,
                                Subject = Constants.SubjectChangeMeetingDataEmail
                            }, Constants.TitleChangeMeetingDataEmail,
                            $"{Constants.ContentChangeMeetingDataEmail}: {changes}",
                            Constants.ButtonCheckMeeting,
                            appLink)
                        .ContinueWith(t =>
                        {
                            if (t.Result.Successful) return;
                            foreach (var error in t.Result.Errors)
                            {
                                _logger.LogError(error);
                            }
                        }, TaskScheduler.Default);
                }

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
            return _meetingRepository
                .GetGamesFromMeeting(meetingId)
                .Select(g => g.GameId)
                .ToList();
        }

        private string CompareMeetingData(Meeting oldMeeting, Meeting newMeeting, string timeZone)
        {
            string changes = null;
            var oldStartDateUtc = DateTime.SpecifyKind(oldMeeting.StartDateTime, DateTimeKind.Utc);
            var newStartDateUtc = DateTime.SpecifyKind(newMeeting.StartDateTime, DateTimeKind.Utc);
            if (oldMeeting.StartDateTime != newMeeting.StartDateTime)
            {
                changes += string.Join(", ",
                    $"{Constants.OldValueMeetingMessage} Start date: {ToolsExtensions.ConvertToTimeZoneFromUtc(oldStartDateUtc, timeZone, _logger).ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture)}, " +
                    $"{Constants.CurrentValueMeetingMessage} Start date: {ToolsExtensions.ConvertToTimeZoneFromUtc(newStartDateUtc, timeZone, _logger).ToString(Constants.DateTimeFormat, CultureInfo.InvariantCulture)}, ");
            }

            if (!ToolsExtensions.IfStringsEqual(oldMeeting.Street, newMeeting.Street))
            {
                changes += string.Join(", ",
                    $"{Constants.OldValueMeetingMessage} Street (address): {oldMeeting.Street}, " +
                    $"{Constants.CurrentValueMeetingMessage} Street (address): {newMeeting.Street}, ");
            }

            if (!ToolsExtensions.IfStringsEqual(oldMeeting.City, newMeeting.City))
            {
                changes += string.Join(", ",
                    $"{Constants.OldValueMeetingMessage} City (address): {oldMeeting.City}, " +
                    $"{Constants.CurrentValueMeetingMessage} City (address): {newMeeting.City}");
            }

            return changes;
        }
    }
}