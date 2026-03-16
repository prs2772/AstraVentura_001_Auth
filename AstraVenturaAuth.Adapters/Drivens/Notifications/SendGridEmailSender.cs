using AstraVenturaAuth.Core.Ports.Drivens;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace AstraVenturaAuth.Adapters.Drivens.Notifications;

public class SendGridEmailSender : IEmailSender
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SendGridEmailSender> _logger;

    public SendGridEmailSender(IConfiguration configuration, ILogger<SendGridEmailSender> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendPasswordResetEmailAsync(
        string toEmail,
        string resetLink,
        CancellationToken ct
    )
    {
        try
        {
            var apiKey = _configuration["SendGrid:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                _logger.LogWarning("SendGrid API Key is missing. Email will not be sent.");
                return;
            }

            var client = new SendGridClient(apiKey);
            var fromEmail = _configuration["SendGrid:FromEmail"] ?? "no-reply@astraventura.com";
            var fromName = _configuration["SendGrid:FromName"] ?? "AstraVentura Support";

            var from = new EmailAddress(fromEmail, fromName);
            var subject = "Password Reset Request";
            var to = new EmailAddress(toEmail);
            var plainTextContent =
                $"You have requested a password reset. Please use the following link to reset your password: {resetLink}\n\nThis link will expire in 15 minutes.";
            var htmlContent =
                $"<p>You have requested a password reset. Please use the following link to reset your password:</p><p><a href=\"{resetLink}\">{resetLink}</a></p><p>This link will expire in 15 minutes.</p>";

            var msg = MailHelper.CreateSingleEmail(
                from,
                to,
                subject,
                plainTextContent,
                htmlContent
            );
            var response = await client.SendEmailAsync(msg, ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    $"SendGrid failed to send email. Status Code: {response.StatusCode}"
                );
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while dispatching the SendGrid email.");
        }
    }
}
