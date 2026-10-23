using MailKit.Net.Smtp; // Import MailKit SMTP client namespace for sending emails over network
using Microsoft.Extensions.Configuration; // Import configuration namespace to read appsettings.json values
using MimeKit; // Import MimeKit namespace for constructing email messages and HTML bodies

namespace TaskFlowPro.Infrastructure.Services // Define infrastructure namespace for backend services
{
    public interface IEmailService // Define interface for email operations enabling dependency injection
    {
        Task SendVerificationEmailAsync(string email, string confirmationToken); // Method signature for sending verification email
    }

    public class EmailService : IEmailService // Define EmailService class implementing IEmailService interface
    {
        private readonly IConfiguration _configuration; // Declare private read-only configuration field

        public EmailService(IConfiguration configuration) // Constructor injecting configuration via dependency injection
        {
            _configuration = configuration; // Assign injected configuration instance to private field
        }

        public async Task SendVerificationEmailAsync(string email, string confirmationToken) // Asynchronous implementation to send email
        {
            var emailSettings = _configuration.GetSection("EmailSettings"); // Retrieve EmailSettings section from appsettings.json
            var server = emailSettings["SmtpServer"] ?? "smtp.gmail.com"; // Get SMTP server string (smtp.gmail.com)
            var port = int.Parse(emailSettings["SmtpPort"] ?? "587"); // Get SMTP port number (587 for TLS)
            var senderEmail = emailSettings["SenderEmail"] ?? throw new InvalidOperationException("SenderEmail is not configured."); // Get sender Gmail address
            var senderName = emailSettings["SenderName"] ?? "TaskFlow Pro"; // Get sender display name
            var senderPassword = emailSettings["SenderPassword"] ?? throw new InvalidOperationException("SenderPassword is not configured."); // Get Google App Password

            var message = new MimeMessage(); // Instantiate new MimeMessage email object
            message.From.Add(new MailboxAddress(senderName, senderEmail)); // Add sender display name and email address
            message.To.Add(new MailboxAddress("", email)); // Add recipient user's real Gmail address
            message.Subject = "Verify your TaskFlow Pro Account"; // Set email subject line

            // Build HTML body containing the verification link with token pointing to your frontend
            var verificationLink = $"http://localhost:5173/verify-email?token={confirmationToken}&email={email}"; // Construct frontend verification URL
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $"<h2>Welcome to TaskFlow Pro!</h2><p>Please verify your real Gmail account by clicking the link below:</p><a href='{verificationLink}'>Verify Email</a>"
            };

            message.Body = bodyBuilder.ToMessageBody(); // Assign constructed HTML body to message body property

            using var client = new SmtpClient(); // Instantiate MailKit SMTP client using a using statement for automatic memory disposal
            
            // Bypass certificate revocation errors that can occur on local environments
            client.ServerCertificateValidationCallback = (s, c, h, e) => true;

            try // Begin try block for handling SMTP network connection
            {
                await client.ConnectAsync(server, port, MailKit.Security.SecureSocketOptions.StartTls); // Asynchronously connect to Gmail SMTP server using STARTTLS encryption
                await client.AuthenticateAsync(senderEmail, senderPassword); // Asynchronously authenticate using sender Gmail address and App Password
                await client.SendAsync(message); // Asynchronously dispatch the email message across the network
            }
            catch (Exception ex) // Catch block for handling connection or authentication failures
            {
                Console.WriteLine($"Failed to send email: {ex.Message}"); // Log error details to console output
                throw; // Rethrow exception so calling method knows it failed
            }
            finally // Finally block executing after try/catch to ensure clean socket disconnection
            {
                await client.DisconnectAsync(true); // Asynchronously disconnect from SMTP server gracefully
            }
        }
    }
}