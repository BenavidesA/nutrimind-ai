using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Application.Interfaces.Services;

public interface IEmailService
{
    Task SendPasswordResetCodeAsync(string toEmail, string code, CancellationToken cancellationToken = default);
}
