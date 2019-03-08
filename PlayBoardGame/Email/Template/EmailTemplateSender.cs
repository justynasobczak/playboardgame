using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using PlayBoardGame.Email.SendGrid;

namespace PlayBoardGame.Email.Template
{
    public class EmailTemplateSender : IEmailTemplateSender
    {
        private IEmailSender _sender;

        public EmailTemplateSender(IEmailSender sender)
        {
            _sender = sender;
        }

        public async Task<SendEmailResponse> SendGeneralEmailAsync(SendEmailDetails details, string title, string content, string buttonText, string buttonUrl)
        {
            string templateText;// = default(string);
            using (var reader = new StreamReader(Assembly.GetEntryAssembly()
                .GetManifestResourceStream("PlayBoardGame.Email.Template.mailTemplate.html"), Encoding.UTF8))
            {
                templateText = await reader.ReadToEndAsync();
            }

            templateText = templateText.Replace("--Title--", title)
                .Replace("--Content--", content)
                .Replace("--ButtonText--", buttonText)
                .Replace("--ButtonUrl--", buttonUrl);

            details.Content = templateText;

            return await _sender.SendEmailAsync(details);
        }
    }
}
