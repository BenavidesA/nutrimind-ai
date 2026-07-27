#nullable enable
using FluentValidation;
using NutriMind.Application.DTOs.MealPlans;
using System;

namespace NutriMind.Application.Validators.MealPlans;

public class CreateMealPlanDtoValidator : AbstractValidator<CreateMealPlanDto>
{
    public CreateMealPlanDtoValidator()
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

        RuleForEach(x => x.PlannedMeals).SetValidator(new CreatePlannedMealDtoValidator());
    }
}

public class CreatePlannedMealDtoValidator : AbstractValidator<CreatePlannedMealDto>
{
    public CreatePlannedMealDtoValidator()
    {
        RuleFor(x => x.Day)
            .NotEmpty().WithMessage("La fecha de la comida es obligatoria.");

        RuleFor(x => x.QuantityG)
            .GreaterThan(0).WithMessage("La cantidad en gramos debe ser mayor a 0.");

        RuleFor(x => x.MealTypeId)
            .InclusiveBetween(1, 4).WithMessage("El tipo de comida seleccionado no es válido.");

        RuleFor(x => x.FoodId)
            .NotEmpty().WithMessage("El alimento es obligatorio.");
    }
}