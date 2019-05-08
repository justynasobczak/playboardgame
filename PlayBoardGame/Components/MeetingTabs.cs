using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class MeetingTabs : ViewComponent
    {
        public IViewComponentResult Invoke(int meetingId, TabName tabName)
        {
            var vm = new MeetingTabsViewModel { MeetingId = meetingId, SelectedTab = tabName};
            return View(vm);
        }
    }
}