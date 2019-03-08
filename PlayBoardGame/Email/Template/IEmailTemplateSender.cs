using System.Threading.Tasks;
using PlayBoardGame.Email.SendGrid;

namespace PlayBoardGame.Email.Template
{
    public interface IEmailTemplateSender
    {
        Task<SendEmailResponse> SendGeneralEmailAsync(SendEmailDetails details, string title, string content, string buttonText, string buttonUrl);
    }
}
