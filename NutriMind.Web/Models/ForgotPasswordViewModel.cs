using System.ComponentModel.DataAnnotations;

namespace NutriMind.Web.Models;

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "El correo es obligatorio.")]
    [EmailAddress(ErrorMessage = "Ingresa un correo válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}
