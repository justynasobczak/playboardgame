using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class BeforeLoginNav : ViewComponent
    {
        public IViewComponentResult Invoke(BeforeLoginPageName pageName)
        {
            var vm = new BeforeLoginNavViewModel { PageName = pageName};
            return View(vm);
        }
    }
}