namespace PlayBoardGame.Models.ViewModels
{
    public class BeforeLoginNavViewModel
    {
        public BeforeLoginPageName PageName;
    }

    public enum BeforeLoginPageName
    {
        Login,
        Register,
        ResetPassword
    }
}