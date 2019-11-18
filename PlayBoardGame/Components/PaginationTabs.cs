using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class PaginationTabs : ViewComponent
    {
        public IViewComponentResult Invoke(bool hasPreviousPage, bool hasNextPage, int pageIndex, string actionMethod)
        {
            var vm = new PaginationTabsViewModel
                {HasPreviousPage = hasPreviousPage, HasNextPage = hasNextPage, PageIndex = pageIndex, ActionMethod = actionMethod};
            return View(vm);
        }
    }
}