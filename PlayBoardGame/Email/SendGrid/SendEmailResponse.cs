using System.Collections.Generic;

namespace PlayBoardGame.Email.SendGrid
{
    public class SendEmailResponse
    {
        public bool Successful => !(Errors?.Count > 0);

        public List<string> Errors { get; set; }
    }
}