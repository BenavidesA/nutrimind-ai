using FluentValidation;
using NutriMind.Application.DTOs.Foods;

namespace NutriMind.Application.Validators.Foods
{
    public class CreateFoodLogDtoValidator : AbstractValidator<CreateFoodLogDto>
    {
        public CreateFoodLogDtoValidator()
        {
            RuleFor(x => x.FoodId)
                .NotEmpty().WithMessage("El alimento es obligatorio.");

            RuleFor(x => x.MealTypeId)
                .NotEmpty().WithMessage("El tipo de comida es obligatorio.");

            RuleFor(x => x.LogDate)
                .NotEmpty().WithMessage("La fecha es obligatoria.");

            RuleFor(x => x.QuantityG)
                .GreaterThan(0).WithMessage("La cantidad debe ser mayor a 0 gramos.")
                .LessThanOrEqualTo(10000).WithMessage("La cantidad no puede exceder los 10,000 gramos.");

            RuleFor(x => x.Notes)
                .MaximumLength(500).WithMessage("Las notas no pueden exceder los 500 caracteres.");
        }
    }
}
