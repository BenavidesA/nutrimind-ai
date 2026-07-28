using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Application.Settings;
using Resend;

namespace NutriMind.Application.Services;

public class ResendEmailService : IEmailService
{
    private readonly IResend _resend;
    private readonly ResendSettings _settings;
    private readonly ILogger<ResendEmailService> _logger;

    public ResendEmailService(IResend resend, IOptions<ResendSettings> settings, ILogger<ResendEmailService> logger)
    {
        _resend = resend;
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendPasswordResetCodeAsync(string toEmail, string code, CancellationToken cancellationToken = default)
    {
        try
        {
            var message = new EmailMessage
            {
                From = _settings.FromEmail,
                Subject = "Tu código de recuperación de NutriMind",
                HtmlBody = $"<p>Tu código de recuperación de contraseña es:</p><h2>{code}</h2><p>Expira en 15 minutos. Si no solicitaste este correo, ignóralo.</p>"
            };

            // Resend in sandbox mode only delivers to the account's verified address,
            // that's why in DEBUG it's redirected there instead of to the real recipient.
#if DEBUG
            string devEmail = _settings.DevRedirectEmail;
            message.To.Add(devEmail);

            _logger.LogInformation("MODO DEV: Correo original para {Original} redirigido a {DevEmail}", toEmail, devEmail);
#else
            message.To.Add(toEmail);
#endif

            await _resend.EmailSendAsync(message, cancellationToken);
        }
        catch (Exception ex)
        {
            // Resend sandbox mode: only delivers to the address the account was registered
            // with. A send failure here is expected during testing — it shouldn't break the
            // "forgot my password" flow (the code was already saved to the DB before
            // reaching this point).
            _logger.LogError(ex, "Error enviando correo de recuperación a {Email}", toEmail);
        }
    }
}