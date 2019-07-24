namespace PlayBoardGame.Models.ViewModels
{
    public class MeetingTabsViewModel
    {
        public int MeetingId { get; set; }
        
        public MeetingTabName SelectedTab { get; set; }
        
        public bool IsCreateMode { get; set; }
    }
    
    public enum MeetingTabName
    {
        Details,
        Users,
        Messages
    }
}