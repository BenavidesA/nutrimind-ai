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

            // Resend en modo sandbox solo entrega a la dirección verificada de la cuenta,
            // por eso en DEBUG se redirige ahí en vez de al destinatario real.
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
            // Modo sandbox de Resend: solo entrega a la dirección con la que se registró la
            // cuenta. Un fallo de envío aquí es esperado durante pruebas — no debe tumbar el
            // flujo de "olvidé mi contraseña" (el código ya quedó guardado en la BD antes de
            // llegar acá).
            _logger.LogError(ex, "Error enviando correo de recuperación a {Email}", toEmail);
        }
    }
}