using FluentValidation;
using NutriMind.Application.DTOs.Auth;

namespace NutriMind.Application.Validators.Auth;

public class VerifyResetCodeDtoValidator : AbstractValidator<VerifyResetCodeDto>
{
    public VerifyResetCodeDtoValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo es obligatorio.")
            .EmailAddress().WithMessage("El correo no tiene un formato válido.");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("El código es obligatorio.")
            .Length(6).WithMessage("El código debe tener 6 dígitos.");
    }
}
