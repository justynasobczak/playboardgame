namespace PlayBoardGame.Models.ViewModels
{
    public class PaginationTabsViewModel
    {
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }
        public int PageIndex { get; set; }
        public string ActionMethod { get; set; }
    }
}