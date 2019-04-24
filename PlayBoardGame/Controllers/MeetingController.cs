using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using System.Linq;

namespace PlayBoardGame.Controllers
{
    public class MeetingController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;

        public MeetingController(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }
        public ViewResult Index() => View("Calendar");
        
    }
}