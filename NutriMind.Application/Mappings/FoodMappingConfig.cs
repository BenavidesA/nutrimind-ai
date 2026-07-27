using Mapster;

namespace NutriMind.Application.Mappings
{
    public class FoodMappingConfig : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.NewConfig<NutriMind.Domain.Entities.Food, DTOs.Foods.FoodDto>()
                .Map(dest => dest.CategoryName, src => src.FoodCategory != null ? src.FoodCategory.Name : null);

            config.NewConfig<NutriMind.Domain.Entities.FoodLog, DTOs.Foods.FoodLogResponseDto>()
                .Map(dest => dest.FoodName, src => src.Food != null ? src.Food.Name : string.Empty)
                .Map(dest => dest.MealTypeName, src => src.MealType != null ? src.MealType.Name : string.Empty);
        }
    }
}
