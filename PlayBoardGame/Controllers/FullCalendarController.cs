using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using  System.Collections.Generic;

namespace PlayBoardGame.Controllers
{
    [Route("api/[controller]")]
    public class FullCalendarController : Controller
    {
        private readonly IEventRepository _eventRepository;
        public FullCalendarController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        [HttpGet]
        public IEnumerable<Event> Get()
        {
           return _eventRepository.Events;  
        }
    }
}