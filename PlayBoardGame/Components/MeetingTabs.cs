using System;
using Microsoft.AspNetCore.Mvc;

namespace PlayBoardGame.Components
{
    public class MeetingTabs : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var id = RouteData.Values["id"] as string;
            var meetingId = Int32.Parse(id);
            return View(meetingId);
        }  
    }
}