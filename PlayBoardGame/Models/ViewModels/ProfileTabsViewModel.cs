namespace PlayBoardGame.Models.ViewModels
{
    public class ProfileTabsViewModel
    {
        public ProfileTabName SelectedTab { get; set; }
    }
    
    public enum ProfileTabName
    {
        Details,
        Password
    }
}