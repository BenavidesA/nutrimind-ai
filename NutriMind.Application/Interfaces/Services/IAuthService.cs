using System;
using System.Threading;
using System.Threading.Tasks;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Auth;

namespace NutriMind.Application.Interfaces.Services
{
    public interface IAuthService
    {
        Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponseDto>> LoginAsync(LoginDto request, CancellationToken cancellationToken = default);
        Task<Result<AuthResponseDto>> RefreshTokenAsync(RefreshTokenRequestDto request, CancellationToken cancellationToken = default);
        Task<Result<bool>> LogoutAsync(string userId, CancellationToken cancellationToken = default);
        Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto request, CancellationToken cancellationToken = default);
        Task<Result<bool>> VerifyResetCodeAsync(VerifyResetCodeDto request, CancellationToken cancellationToken = default);
        Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto request, CancellationToken cancellationToken = default);
        Task<Result<bool>> ChangePasswordAsync(Guid userId, ChangePasswordDto request, CancellationToken cancellationToken = default);
    }
}