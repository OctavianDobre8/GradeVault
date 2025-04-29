using Microsoft.Extensions.Options;
using System.Net;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace GradeVault.Server.Services
{
    /**
     * @brief Configuration settings for email service
     *
     * Contains all the necessary configuration parameters for connecting to
     * and authenticating with an SMTP server for sending emails.
     */
    public class EmailSettings
    {
        /**
         * @brief SMTP server address
         */
        public string SmtpServer { get; set; }
        
        /**
         * @brief SMTP server port number
         */
        public int SmtpPort { get; set; }
        
        /**
         * @brief Username for SMTP authentication
         */
        public string SmtpUsername { get; set; } // Note: Changed from SenderEmail to SmtpUsername
        
        /**
         * @brief Email address used in the From field
         */
        public string SenderEmail { get; set; }
        
        /**
         * @brief Display name used in the From field
         */
        public string SenderName { get; set; }
        
        /**
         * @brief Password for SMTP authentication
         */
        public string SmtpPassword { get; set; } // Note: Changed from Password to SmtpPassword
        
        /**
         * @brief Flag to enable SSL/TLS for SMTP connection
         */
        public bool EnableSsl { get; set; }
    }

    /**
     * @brief Interface for email sending service
     *
     * Defines the contract for any class that provides email sending functionality.
     */
    public interface IEmailService
    {
        /**
         * @brief Sends an email asynchronously
         *
         * @param email Recipient's email address
         * @param subject Subject line of the email
         * @param message HTML body content of the email
         * @return Task representing the asynchronous operation
         */
        Task SendEmailAsync(string email, string subject, string message);
    }

    /**
     * @brief Service for sending emails using SMTP
     *
     * Implementation of the email service interface that sends emails
     * using the MailKit library and configured SMTP settings.
     */
    public class EmailService : IEmailService
    {
        /**
         * @brief Email configuration settings
         */
        private readonly EmailSettings _emailSettings;
        
        /**
         * @brief Logger for recording email operations
         */
        private readonly ILogger<EmailService> _logger;

        /**
         * @brief Constructor that initializes the email service with configuration
         *
         * @param emailSettings Options containing email configuration settings
         * @param logger Logger for recording operations and errors
         */
        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

        /**
         * @brief Sends an email to a specified recipient
         *
         * Creates and sends an email with HTML content to the specified recipient
         * using the configured SMTP server settings.
         *
         * @param email Recipient's email address
         * @param subject Subject line of the email
         * @param message HTML body content of the email
         * @return Task representing the asynchronous send operation
         * @throws Exception When email sending fails
         */
        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                // Create a new MimeMessage
                var emailMessage = new MimeMessage();
                emailMessage.From.Add(new MailboxAddress(_emailSettings.SenderName, _emailSettings.SenderEmail));
                emailMessage.To.Add(new MailboxAddress("", email));
                emailMessage.Subject = subject;

                // Create the HTML body
                var bodyBuilder = new BodyBuilder { HtmlBody = message };
                emailMessage.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                
                // Log connection attempt
                _logger.LogInformation("Attempting to connect to SMTP server: {Server}:{Port}", 
                    _emailSettings.SmtpServer, _emailSettings.SmtpPort);
                
                // Connect with appropriate security options
                await client.ConnectAsync(
                    _emailSettings.SmtpServer,
                    _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.Auto);

                // Log connection status
                _logger.LogInformation("SMTP connection established. SSL: {EnableSsl}", _emailSettings.EnableSsl);
                
                // Check if we need to authenticate
                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername) && !string.IsNullOrEmpty(_emailSettings.SmtpPassword))
                {
                    _logger.LogInformation("Authenticating with SMTP server using username: {Username}", 
                        _emailSettings.SmtpUsername);
                    
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword);
                    _logger.LogInformation("SMTP authentication successful");
                }

                // Send the email
                _logger.LogInformation("Sending email to: {Email}", email);
                await client.SendAsync(emailMessage);
                
                // Disconnect properly
                await client.DisconnectAsync(true);
                
                _logger.LogInformation("Email sent successfully to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}: {Message}", email, ex.Message);
                throw;
            }
        }
    }
}