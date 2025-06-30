using Microsoft.Extensions.Configuration;
using SendGrid.Helpers.Mail;
using SendGrid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BookLendingBackUp.Application.Interfaces;

namespace BookLendingBackUp.Infrastructure.Services
{
    public class SendGridEmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public SendGridEmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_config["SendGrid:SenderEmail"], _config["SendGrid:SenderName"]);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);
            await client.SendEmailAsync(msg);
        }
    }
}
