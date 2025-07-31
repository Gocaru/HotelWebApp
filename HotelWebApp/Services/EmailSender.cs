using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace HotelWebApp.Services
{
    /// <summary>
    /// Implements the IEmailSender interface from ASP.NET Core Identity to send emails.
    /// This service uses MailKit library to connect to an SMTP server and dispatch emails
    /// for functionalities like account confirmation and password reset.
    /// </summary>
    public class EmailSender : IEmailSender
    {
        private readonly EmailSettings _emailSettings;

        /// <summary>
        /// Initializes a new instance of the EmailSender class.
        /// </summary>
        /// <param name="emailSettings">The email configuration settings, injected via IOptions.</param>
        public EmailSender(IOptions<EmailSettings> emailSettings)
        {
            _emailSettings = emailSettings.Value;
        }

        /// <summary>
        /// Constructs and sends an email asynchronously.
        /// </summary>
        /// <param name="email">The recipient's email address.</param>
        /// <param name="subject">The subject line of the email.</param>
        /// <param name="htmlMessage">The content of the email, formatted as an HTML string.</param>
        /// <returns>A task that represents the asynchronous send operation.</returns>
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Create a new MimeMessage object which is the core object from MailKit/MimeKit.
            var emailMessage = new MimeMessage();

            // Set the sender and recipient addresses.
            emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
            emailMessage.To.Add(new MailboxAddress("", email)); // Recipient name is left empty.
            emailMessage.Subject = subject;

            // Use BodyBuilder to easily create an HTML email body.
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // Connect to the SMTP server using the configured settings.
                // SecureSocketOptions.StartTls is a common and secure way to connect.
                await client.ConnectAsync(_emailSettings.SmtpServer, _emailSettings.Port, SecureSocketOptions.StartTls);

                // Authenticate with the SMTP server using the sender's credentials.
                await client.AuthenticateAsync(_emailSettings.SenderEmail, _emailSettings.Password);

                // Send the email.
                await client.SendAsync(emailMessage);

                // Disconnect cleanly from the server.
                await client.DisconnectAsync(true);
            }
        }
    }
}
