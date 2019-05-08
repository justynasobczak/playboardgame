namespace PlayBoardGame.Models.ViewModels
{
    public class MeetingTabsViewModel
    {
        public int MeetingId { get; set; }
        
        public TabName SelectedTab { get; set; }  
    }
    
    public enum TabName
    {
        Details,
        Users
    }
}