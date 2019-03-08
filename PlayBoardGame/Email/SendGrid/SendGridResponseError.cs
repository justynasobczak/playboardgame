namespace PlayBoardGame.Email.SendGrid
{
    public class SendGridResponseError
    {
        //The error message
        public string Message { get; set; }

        //The field inside the email message details that the error is related to
        public string Field { get; set; }

        //Useful information for resolving the error
        public string Help { get; set; }
    }
}
