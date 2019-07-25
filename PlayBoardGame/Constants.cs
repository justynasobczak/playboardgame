namespace PlayBoardGame
{
    public static class Constants
    {
        //Messages
        public const string GeneralSuccessMessage = "The operation succeeded.";
        public const string GeneralSendEmailErrorMessage = "Email was not sent, please contact support.";
        public const string UserOrPasswordErrorMessage = "Invalid user or password";
        public const string SendResetLinkSuccessMessage = "Email with the link for resetting password was sent. Please check your inbox.";
        public const string LackOfEmailMatchMessage = "Entered email address doesn't match any account.";
        public const string GeneralResetPasswordErrorMessage = "Please contact support because of unknown error during resetting the password.";
        public const string WrongOldPasswordMessage = "Invalid old password";
        public const string NotValidTokenMessage = "Invalid token";
        public const string FutureDateInPastMessage = "Please enter a date in the future.";
        public const string EndDateBeforeStartMessage = "Please enter a date later than start date.";
        public const string OverlappingMeetingsMessage = "There are some overlapping meetings";
        
        //Logs
        public const string UnknownId = "Unknown Id";
        public const string UnknownError = "Unknown error occurred";
    }
}
