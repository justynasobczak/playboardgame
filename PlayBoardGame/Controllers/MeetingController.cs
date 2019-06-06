using System;
using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using System.Linq;
using System.Threading.Tasks;
using PlayBoardGame.Models.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace PlayBoardGame.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        private readonly UserManager<AppUser> _userManager;

        public MeetingController(IMeetingRepository meetingRepository, UserManager<AppUser> userManager)
        {
            _meetingRepository = meetingRepository;
            _userManager = userManager;
        }

        public ViewResult List() => View("Calendar");

        public IActionResult Edit(int id)
        {
            var currentUserId = GetCurrentUserId().Result;
            var meeting = _meetingRepository.Meetings.FirstOrDefault(m => m.MeetingID == id);
            if (meeting != null)
            {
                var vm = new MeetingViewModels.CreateEditMeetingViewModel
                {
                    Organizers = _userManager.Users.ToList(),
                    OrganizerId = meeting.Organizer.Id,
                    Title = meeting.Title,
                    MeetingID = meeting.MeetingID,
                    StartDateTime = meeting.StartDateTime,
                    EndDateTime = meeting.EndDateTime,
                    Notes = meeting.Notes,
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

            return RedirectToAction("Error", "Error");
        }


        [HttpPost]
        public async Task<IActionResult> Edit(MeetingViewModels.CreateEditMeetingViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(vm.OrganizerId);
                var meeting = new Meeting
                {
                    MeetingID = vm.MeetingID,
                    Title = vm.Title,
                    StartDateTime = vm.StartDateTime,
                    EndDateTime = vm.EndDateTime,
                    Organizer = user,
                    Street = vm.Address.Street,
                    PostalCode = vm.Address.PostalCode,
                    City = vm.Address.City,
                    Country = vm.Address.Country,
                    Notes = vm.Notes
                };
                _meetingRepository.SaveMeeting(meeting);
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction("Edit", new {id = meeting.MeetingID});
            }

            vm.Organizers = _userManager.Users.ToList();
            return View(vm);
        }

        public ViewResult Create()
        {
            var currentUserId = GetCurrentUserId().Result;
            return View("Edit", new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizers = _userManager.Users.ToList(),
                OrganizerId = currentUserId,
                StartDateTime = DateTime.Now,
                EndDateTime = DateTime.Now.AddHours(1),
                IsEditable = true
            });
        }

        private async Task<string> GetCurrentUserId()
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            return user.Id;
        }
    }
}