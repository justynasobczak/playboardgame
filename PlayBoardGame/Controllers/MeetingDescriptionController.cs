using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using PlayBoardGame.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace PlayBoardGame.Controllers
{
    [Authorize]
    public class MeetingDescriptionController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;

        public MeetingDescriptionController(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        public IActionResult Edit(int id)
        {
            if (id == 0)
            {
                return RedirectToAction(nameof(MeetingController.List), "Meeting");
            }

            var description = _meetingRepository.GetDescriptionFromMeeting(id);

            return View(new MeetingDescriptionViewModel {MeetingId = id, Description = description});
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(MeetingDescriptionViewModel vm)
        {
            if (!ModelState.IsValid) return View(vm);
            _meetingRepository.SaveDescriptionForMeeting(vm.Description, vm.MeetingId);
            TempData["SuccessMessage"] = Constants.GeneralSuccessMessage;
            return RedirectToAction(nameof(Edit), new {id = vm.MeetingId});

        }
    }
}