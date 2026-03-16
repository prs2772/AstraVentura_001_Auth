namespace AstraVenturaAuth.Core.Ports.Drivens;

public interface IEmailSender
{
    Task SendPasswordResetEmailAsync(string toEmail, string resetLink, CancellationToken ct);
}
