using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;

namespace HotelWebApp.Services
{
    public class EmailSenderSmtp : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        public EmailSenderSmtp(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"=== SENDING EMAIL VIA SMTP ===");
                System.Diagnostics.Debug.WriteLine($"To: {email}");
                System.Diagnostics.Debug.WriteLine($"Subject: {subject}");
                System.Diagnostics.Debug.WriteLine($"SMTP: {_emailSettings.SmtpServer}:{_emailSettings.Port}");
                System.Diagnostics.Debug.WriteLine($"From: {_emailSettings.SenderEmail}");

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(
                        _emailSettings.SenderEmail ?? "noreply@hotelwebapp.com",
                        _emailSettings.SenderName ?? "HotelWebApp"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);

                using (var smtpClient = new SmtpClient(
                    _emailSettings.SmtpServer ?? "smtp.gmail.com",
                    _emailSettings.Port))
                {
                    smtpClient.Credentials = new NetworkCredential(
                        _emailSettings.SenderEmail,
                        _emailSettings.Password);
                    smtpClient.EnableSsl = true;
                    smtpClient.Timeout = 30000; // 30 segundos

                    await smtpClient.SendMailAsync(mailMessage);
                }

                System.Diagnostics.Debug.WriteLine("Email sent successfully via SMTP!");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SMTP Error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException?.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }
    }
}