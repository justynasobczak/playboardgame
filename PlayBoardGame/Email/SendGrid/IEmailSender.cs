using System.Threading.Tasks;

namespace PlayBoardGame.Email.SendGrid
{
    public interface IEmailSender
    {
        Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details);
    }
}
