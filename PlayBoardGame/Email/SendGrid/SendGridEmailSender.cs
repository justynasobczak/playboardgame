using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PlayBoardGame.Email.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {

        private IConfiguration _configuration;

        public SendGridEmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details)
        {
            var apiKey = _configuration["SendGridKey"];

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(details.FromEmail, details.FromName);

            var to = new EmailAddress(details.ToEmail, details.ToName);

            var subject = details.Subject;

            var content = details.Content;

            var msg = MailHelper.CreateSingleEmail(from, to, subject, details.IsHTML ? null : details.Content,
                details.IsHTML ? details.Content : null);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                return new SendEmailResponse();
            }

            try
            {
                var bodyResult = await response.Body.ReadAsStringAsync();
                var sendGridResponse = JsonConvert.DeserializeObject<SendGridResponse>(bodyResult);

                var errorResponse = new SendEmailResponse
                {
                    Errors = sendGridResponse?.Errors.Select(f => f.Message).ToList()
                };

                if (errorResponse.Errors == null || errorResponse.Errors.Count == 0)
                {
                    errorResponse.Errors = new List<string>(new[] { "Unknow error from email sending server. Please contact support." });
                }
                return errorResponse;
            }
            catch (Exception ex)
            {
                if (Debugger.IsAttached)
                {
                    var error = ex;
                    Debugger.Break();
                }
                return new SendEmailResponse
                {
                    Errors = new List<string>(new[] { "Unknown error occurred" })
                };
            }
        }
    }
}

