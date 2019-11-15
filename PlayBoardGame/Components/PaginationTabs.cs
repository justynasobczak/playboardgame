using Microsoft.AspNetCore.Mvc;
using PlayBoardGame.Models.ViewModels;

namespace PlayBoardGame.Components
{
    public class PaginationTabs : ViewComponent
    {
        public IViewComponentResult Invoke(bool hasPreviousPage, bool hasNextPage, int pageIndex)
        {
            var vm = new PaginationTabsViewModel
                {HasPreviousPage = hasPreviousPage, HasNextPage = hasNextPage, PageIndex = pageIndex};
            return View(vm);
        }
    }
}