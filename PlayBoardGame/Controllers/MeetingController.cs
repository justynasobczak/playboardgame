using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using System.Linq;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;

        public MeetingController(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }
        public ViewResult List() => View("Calendar");
        
        public IActionResult Edit(int id)
        {
            var meeting = _meetingRepository.Meetings.FirstOrDefault(m => m.MeetingID == id);
            if (meeting != null)
            {
                var vm = new MeetingViewModels.CreateEditMeetingViewModel
                {
                    Title = meeting.Title,
                    MeetingID = meeting.MeetingID,
                    StartDateTime = meeting.StartDateTime,
                    EndDateTime = meeting.EndDateTime
                };
                return View(vm);
            }
            return RedirectToAction("Error", "Error");
        }


        [HttpPost]
        public IActionResult Edit(MeetingViewModels.CreateEditMeetingViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var meeting = new Meeting
                {
                    MeetingID = vm.MeetingID,
                    Title = vm.Title,
                    StartDateTime = vm.StartDateTime,
                    EndDateTime = vm.EndDateTime
                };
                _meetingRepository.SaveMeeting(meeting);
                TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
                return RedirectToAction("List");
            }
            return View(vm);
        }

        public ViewResult Create() => View("Edit", new MeetingViewModels.CreateEditMeetingViewModel());

    }
}