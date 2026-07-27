using System.ComponentModel.DataAnnotations;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Models;

public class ProfileIndexViewModel
{
    public ProfileResponseDto? Profile { get; set; }
    public List<BadgeResponseDto> Badges { get; set; } = new();
    public string? ErrorMessage { get; set; }
}

public class EditProfileViewModel
{
    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Range(1, 120, ErrorMessage = "Ingresa una edad válida.")]
    [Display(Name = "Edad")]
    public int? Age { get; set; }

    [Display(Name = "Género")]
    public string? Gender { get; set; }

    [Range(50, 260, ErrorMessage = "Ingresa una estatura válida en cm.")]
    [Display(Name = "Estatura (cm)")]
    public decimal? HeightCm { get; set; }

    [Range(1, 400, ErrorMessage = "Ingresa un peso válido en kg.")]
    [Display(Name = "Peso (kg)")]
    public decimal? WeightKg { get; set; }

    [Display(Name = "Nivel de actividad")]
    public ActivityLevel ActivityLevel { get; set; }

    [Display(Name = "Objetivo")]
    public DietaryGoal DietaryGoal { get; set; }

    [Display(Name = "Universidad")]
    public string? University { get; set; }

    public string? ErrorMessage { get; set; }
}

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Ingresa tu contraseña actual.")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña actual")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Ingresa tu nueva contraseña.")]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [DataType(DataType.Password)]
    [Display(Name = "Nueva contraseña")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirma tu nueva contraseña.")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar nueva contraseña")]
    public string ConfirmNewPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }
}
