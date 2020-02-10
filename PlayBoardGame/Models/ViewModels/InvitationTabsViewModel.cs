namespace PlayBoardGame.Models.ViewModels
{
    public class InvitationTabsViewModel
    {
        public InvitationTabName SelectedTab { get; set; }
    }

    public enum InvitationTabName
    {
        Sent,
        Received
    }
}