using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class MeetingTabs : ViewComponent
    {
        public IViewComponentResult Invoke(int meetingId, MeetingTabName tabName, bool isCreateMode)
        {
            var vm = new MeetingTabsViewModel { MeetingId = meetingId, SelectedTab = tabName, IsCreateMode = isCreateMode};
            return View(vm);
        }
    }
}