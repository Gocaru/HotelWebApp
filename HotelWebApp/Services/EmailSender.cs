using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace HotelWebApp.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _configuration["EmailSettings:ApiKey"];
            var fromEmail = _configuration["EmailSettings:FromEmail"];
            var fromName = _configuration["EmailSettings:FromName"];

            System.Diagnostics.Debug.WriteLine($"=== SENDGRID EMAIL ===");
            System.Diagnostics.Debug.WriteLine($"To: {email}");
            System.Diagnostics.Debug.WriteLine($"From: {fromEmail} ({fromName})");
            System.Diagnostics.Debug.WriteLine($"Subject: {subject}");

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

            var response = await client.SendEmailAsync(msg);

            System.Diagnostics.Debug.WriteLine($"SendGrid StatusCode: {response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                var body = await response.Body.ReadAsStringAsync();
                System.Diagnostics.Debug.WriteLine($"SendGrid Error: {body}");
                throw new Exception($"SendGrid failed: {response.StatusCode} - {body}");
            }

            System.Diagnostics.Debug.WriteLine("Email sent successfully via SendGrid!");
        }
    }
}