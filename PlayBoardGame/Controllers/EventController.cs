using Microsoft.AspNetCore.Mvc;

namespace PlayBoardGame.Controllers
{
    public class EventController : Controller
    {
        public ViewResult Index() => View("Calendar");
    }
}