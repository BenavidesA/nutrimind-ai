namespace NutriMind.Web.Services.Dtos;

public class VerifyResetCodeRequestDto
{
    public string Email { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}
