using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public interface IAuthApiService
{
    Task<ApiResult<AuthResponseDto>> LoginAsync(LoginRequestDto request);
    Task<ApiResult<AuthResponseDto>> RegisterAsync(RegisterRequestDto request);
    Task<ApiResult> ForgotPasswordAsync(ForgotPasswordRequestDto request);
    Task<ApiResult> VerifyResetCodeAsync(VerifyResetCodeRequestDto request);
    Task<ApiResult> ResetPasswordAsync(ResetPasswordRequestDto request);
    Task<ApiResult> LogoutAsync(string accessToken);
}
