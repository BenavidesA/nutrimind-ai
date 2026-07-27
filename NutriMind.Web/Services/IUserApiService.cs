using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Services;

public interface IUserApiService
{
    Task<ApiResult<ProgressDto>> GetProgressAsync();
    Task<ApiResult<ProfileResponseDto>> GetProfileAsync();
    Task<ApiResult<ProfileResponseDto>> UpdateProfileAsync(UpdateProfileDto request);
    Task<ApiResult> ChangePasswordAsync(ChangePasswordDto request);
    Task<ApiResult> DeleteAccountAsync();
    Task<ApiResult<List<BadgeResponseDto>>> GetBadgesAsync();
}
