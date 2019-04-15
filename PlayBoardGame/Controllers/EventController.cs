using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models;
using System.Linq;

namespace PlayBoardGame.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }
        public ViewResult Index() => View("Calendar");

        public JsonResult GetEvents()
        {
            var events = _eventRepository.Events.ToList();
            return new JsonResult(events);
        }
        
    }
}