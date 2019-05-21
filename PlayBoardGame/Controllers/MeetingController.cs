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
            var meeting = _meetingRepository.Meetings.FirstOrDefault(m => m.MeetingID == id);
            if (meeting != null)
            {
                var vm = new MeetingViewModels.CreateEditMeetingViewModel
                {
                    Organizers = _userManager.Users.ToList(),
                    Organizer = meeting.Organizer.Id,
                    Title = meeting.Title,
                    MeetingID = meeting.MeetingID,
                    StartDateTime = meeting.StartDateTime,
                    EndDateTime = meeting.EndDateTime,
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
                var user = await _userManager.FindByIdAsync(vm.Organizer);
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
                    Country = vm.Address.Country
                };
                _meetingRepository.SaveMeeting(meeting);
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction("Edit", new {id = meeting.MeetingID});
            }

            return View(vm);
        }

        public ViewResult Create()
        {
            return View("Edit", new MeetingViewModels.CreateEditMeetingViewModel
            {
                Organizers = _userManager.Users.ToList()
            });
        }
    }
}