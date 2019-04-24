using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using  System.Collections.Generic;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class FullCalendarController : Controller
    {
        private readonly IMeetingRepository _meetingRepository;
        public FullCalendarController(IMeetingRepository meetingRepository)
        {
            _meetingRepository = meetingRepository;
        }

        [HttpGet]
        public IEnumerable<Meeting> Get()
        {
           return _meetingRepository.Meetings;  
        }
    }
}