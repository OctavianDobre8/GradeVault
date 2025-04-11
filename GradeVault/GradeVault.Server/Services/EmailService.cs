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
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; } // Note: Changed from SenderEmail to SmtpUsername
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SmtpPassword { get; set; } // Note: Changed from Password to SmtpPassword
        public bool EnableSsl { get; set; }
    }

    public interface IEmailService
    {
        Task SendEmailAsync(string email, string subject, string message);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IOptions<EmailSettings> emailSettings, ILogger<EmailService> logger)
        {
            _emailSettings = emailSettings.Value;
            _logger = logger;
        }

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