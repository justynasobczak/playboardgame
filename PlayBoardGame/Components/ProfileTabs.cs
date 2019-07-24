using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class ProfileTabs : ViewComponent
    {
        public IViewComponentResult Invoke(ProfileTabName tabName)
        {
            var vm = new ProfileTabsViewModel {SelectedTab = tabName};
            return View(vm);
        }
    }
}