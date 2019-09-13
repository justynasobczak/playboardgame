﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace PlayBoardGame.Email.SendGrid
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public SendGridEmailSender(IConfiguration configuration, IHostingEnvironment environment)
        {
            _configuration = configuration;
            _environment = environment;
        }

        public async Task<SendEmailResponse> SendEmailAsync(SendEmailDetails details)
        {
            var apiKey = _configuration["SendGridKey"];

            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(_configuration["Data:SendGridFrom:FromEmail"],
                _configuration["Data:SendGridFrom:FromName"]);

            var to = _environment.IsDevelopment()
                ? new EmailAddress(_configuration["Data:SendGridTo:ToEmailForTests"],
                    _configuration["Data:SendGridTo:ToNameForTests"])
                : new EmailAddress(details.ToEmail, details.ToName);
            var subject = details.Subject;

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
                    errorResponse.Errors = new List<string>(new[] {Constants.GeneralSendEmailErrorMessage});
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
                    Errors = new List<string>(new[] {Constants.UnknownError})
                };
            }
        }
    }
}