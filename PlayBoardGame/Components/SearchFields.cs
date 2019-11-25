using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class SearchFields : ViewComponent
    {
        public IViewComponentResult Invoke(string actionMethod)
        {
            var vm = new SearchFieldsViewModel {ActionMethod = actionMethod};
            return View(vm);
        }
    }
}