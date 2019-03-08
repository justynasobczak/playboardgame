using System.Collections.Generic;

namespace PlayBoardGame.Email.SendGrid
{
    public class SendGridResponse
    {
        public List<SendGridResponseError> Errors { get; set; }
    }
}
