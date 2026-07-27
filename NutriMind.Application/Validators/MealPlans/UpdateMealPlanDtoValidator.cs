#nullable enable
using FluentValidation;
using NutriMind.Application.DTOs.MealPlans;

namespace NutriMind.Application.Validators.MealPlans;

public class UpdateMealPlanDtoValidator : AbstractValidator<UpdateMealPlanDto>
{
    public UpdateMealPlanDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("El nombre del plan es obligatorio.")
            .MaximumLength(150).WithMessage("El nombre no puede superar los 150 caracteres.");

        RuleFor(x => x.StartDate)
            .NotEmpty().WithMessage("La fecha de inicio es obligatoria.");

        RuleFor(x => x.EndDate)
            .NotEmpty().WithMessage("La fecha de fin es obligatoria.")
            .GreaterThanOrEqualTo(x => x.StartDate).WithMessage("La fecha de fin no puede ser anterior a la fecha de inicio.");

        RuleFor(x => x.TotalCaloriesPerDay)
            .GreaterThan(0).WithMessage("Las calorías objetivo deben ser mayores a 0.");
    }
}
