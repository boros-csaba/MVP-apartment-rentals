using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;

namespace MVPApartmentRentals.Services
{
    public class SendGridEmailSenderService : IEmailSenderService
    {
        private readonly string apiKey;
        private readonly string fromEmail;
        private readonly string fromName;

        public SendGridEmailSenderService(string apiKey, string fromEmail, string fromName)
        {
            this.apiKey = apiKey ?? throw new ArgumentNullException("apiKey");
            this.fromEmail = fromEmail ?? throw new ArgumentNullException("fromEmail");
            this.fromName = fromName ?? throw new ArgumentNullException("fromName");
        }

        public async Task SendEmailAsync(Email email)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email.ToEmail, email.ToName);
            var message = MailHelper.CreateSingleEmail(from, to, email.Subject, null, email.Content);
            await client.SendEmailAsync(message);
        }
    }
}
