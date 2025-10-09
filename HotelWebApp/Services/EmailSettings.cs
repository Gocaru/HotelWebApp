namespace HotelWebApp.Services
{
    /// <summary>
    /// Represents the configuration settings required to send emails.
    /// Supports both SendGrid API and SMTP configurations.
    /// </summary>
    public class EmailSettings
    {
        // SendGrid settings
        public string? ApiKey { get; set; }
        public string? FromEmail { get; set; }
        public string? FromName { get; set; }

        // SMTP settings
        /// <summary>
        /// The hostname or IP address of the SMTP server (e.g., "smtp.gmail.com").
        /// </summary>
        public string? SmtpServer { get; set; }

        /// <summary>
        /// The port number for the SMTP server (e.g., 587 for TLS).
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// The display name of the email sender (e.g., "HotelWebApp Support").
        /// </summary>
        public string? SenderName { get; set; }

        /// <summary>
        /// The email address used to send emails from (e.g., "noreply@hotelwebapp.com").
        /// This is also typically the username for SMTP authentication.
        /// </summary>
        public string? SenderEmail { get; set; }

        /// <summary>
        /// The password for the sender's email account, used for SMTP authentication.
        /// For security, this value should be stored in a secure location like User Secrets or Azure Key Vault.
        /// </summary>
        public string? Password { get; set; }
    }

}
