#nullable enable
using FluentValidation;
using NutriMind.Application.DTOs.AI;

namespace NutriMind.Application.Validators.AI;

public class AiMealPlanRequestDtoValidator : AbstractValidator<AiMealPlanRequestDto>
{
    public AiMealPlanRequestDtoValidator()
    {
        RuleFor(x => x.Days)
            .InclusiveBetween(1, 30).WithMessage("Los días deben estar entre 1 y 30.");
    }
}
