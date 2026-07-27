using System.ComponentModel.DataAnnotations;

namespace NutriMind.Web.Models;

public class VerifyCodeViewModel
{
    [Required]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa el código que recibiste.")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "El código debe tener 6 dígitos.")]
    [Display(Name = "Código de verificación")]
    public string Code { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
