namespace PlayBoardGame
{
    public static class Constants
    {
        //Messages
        public const string GeneralSuccessMessage = "The operation succeeded.";
        public const string GeneralSendEmailErrorMessage = "Please contact support because of unknown error from email sending server.";
        public const string UserOrPasswordErrorMessage = "Invalid user or password";
        public const string SendResetLinkSuccessMessage = "Email with the link for resetting password was sent. Please check your inbox.";
        public const string LackOfEmailMatchMessage = "Entered email address doesn't match any account";
        public const string GeneralResetPasswordErrorMessage = "Please contact support because of unknown error during resetting the password";
        public const string WrongOldPasswordMessage = "Invalid old password";
        public const string NotValidTokenMessage = "Invalid token";
    }
}
