using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class InvitationTabs : ViewComponent
    {
        public IViewComponentResult Invoke(InvitationTabName tabName)
        {
            var vm = new InvitationTabsViewModel {SelectedTab = tabName};
            return View(vm);
        }
        
    }
}