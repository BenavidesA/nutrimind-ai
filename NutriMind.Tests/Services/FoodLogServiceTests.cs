using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.AI;
using NutriMind.Application.DTOs.Foods;
using NutriMind.Application.Interfaces;
using NutriMind.Application.Mappings;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;
using NutriMind.Infrastructure.Persistence.Services;
using Xunit;

namespace NutriMind.Tests.Services;

public class FoodLogServiceTests
{
    static FoodLogServiceTests()
    {
        // Registra el mismo mapeo Food/FoodLog -> DTO que la app configura en el arranque real
        // (AddApplicationMappings). Sin esto, Adapt<FoodLogResponseDto> no sabe cómo llenar
        // FoodName/MealTypeName y el test dejaría de reflejar el comportamiento de producción.
        TypeAdapterConfig.GlobalSettings.Scan(typeof(FoodMappingConfig).Assembly);
    }

    private readonly IFoodLogRepository _foodLogRepository = Substitute.For<IFoodLogRepository>();
    private readonly IFoodRepository _foodRepository = Substitute.For<IFoodRepository>();
    private readonly IRepository<DailyIntakeSummary> _summaryRepository = Substitute.For<IRepository<DailyIntakeSummary>>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IAiService _aiService = Substitute.For<IAiService>();
    private readonly ILogger<FoodLogService> _logger = Substitute.For<ILogger<FoodLogService>>();

    private FoodLogService CreateSut() => new(
        _foodLogRepository, _foodRepository, _summaryRepository, _unitOfWork, _aiService, _logger);

    // Hace que GetHistoryForUserAsync devuelva el mismo FoodLog que el servicio acaba de pasar
    // a AddAsync, tal como haría una base de datos real inmediatamente después de un insert.
    // Sin esto, el flujo de UpdateDailySummaryAsync y la relectura final fallarían con "not found"
    // porque el Id se genera dentro del método bajo prueba y no se puede conocer de antemano.
    private void CaptureAddedFoodLog()
    {
        FoodLog? added = null;

        _foodLogRepository
            .AddAsync(Arg.Do<FoodLog>(fl => added = fl), Arg.Any<CancellationToken>())
            .Returns(Task.CompletedTask);

        _foodLogRepository
            .GetHistoryForUserAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>(), Arg.Any<int>(), Arg.Any<int>(), Arg.Any<CancellationToken>())
            .Returns(_ => added is null ? Enumerable.Empty<FoodLog>() : new[] { added });
    }

    [Fact]
    public async Task SmartAddFoodLogAsync_FoodAlreadyInCatalog_DoesNotCallAiService()
    {
        var existing = new Food
        {
            Id = Guid.NewGuid(),
            Name = "Manzana",
            CaloriesPer100g = 52,
            ProteinPer100g = 0.3m,
            CarbsPer100g = 14,
            FatPer100g = 0.2m
        };

        _foodRepository
            .SearchFoodsAsync("Manzana", 1, 5, Arg.Any<CancellationToken>())
            .Returns(new[] { existing });

        CaptureAddedFoodLog();

        var sut = CreateSut();
        var dto = new SmartAddFoodLogDto { Name = "Manzana", QuantityG = 150 };

        var result = await sut.SmartAddFoodLogAsync(Guid.NewGuid(), dto);

        Assert.True(result.IsSuccess);
        await _aiService.DidNotReceive().EstimateFoodNutritionAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        await _foodRepository.DidNotReceive().AddAsync(Arg.Any<Food>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SmartAddFoodLogAsync_FoodNotInCatalog_CallsAiServiceAndPersistsEstimate()
    {
        _foodRepository
            .SearchFoodsAsync("Quinoa cocida", 1, 5, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Food>());

        _aiService
            .EstimateFoodNutritionAsync("Quinoa cocida", Arg.Any<CancellationToken>())
            .Returns(Result<FoodEstimateDto>.Success(new FoodEstimateDto
            {
                CaloriesPer100g = 120,
                ProteinPer100g = 4.4m,
                CarbsPer100g = 21.3m,
                FatPer100g = 1.9m
            }));

        CaptureAddedFoodLog();

        var sut = CreateSut();
        var dto = new SmartAddFoodLogDto { Name = "Quinoa cocida", QuantityG = 200 };

        var result = await sut.SmartAddFoodLogAsync(Guid.NewGuid(), dto);

        Assert.True(result.IsSuccess);
        await _aiService.Received(1).EstimateFoodNutritionAsync("Quinoa cocida", Arg.Any<CancellationToken>());
        await _foodRepository.Received(1).AddAsync(
            Arg.Is<Food>(f => f.Source == "AiEstimated" && f.CaloriesPer100g == 120),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SmartAddFoodLogAsync_AiServiceFails_ReturnsFailureAndPersistsNothing()
    {
        _foodRepository
            .SearchFoodsAsync("Alimento raro", 1, 5, Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<Food>());

        _aiService
            .EstimateFoodNutritionAsync("Alimento raro", Arg.Any<CancellationToken>())
            .Returns(Result<FoodEstimateDto>.Failure("Timeout al contactar Gemini."));

        var sut = CreateSut();
        var dto = new SmartAddFoodLogDto { Name = "Alimento raro", QuantityG = 100 };

        var result = await sut.SmartAddFoodLogAsync(Guid.NewGuid(), dto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Timeout al contactar Gemini.", result.ErrorMessage);
        await _foodLogRepository.DidNotReceive().AddAsync(Arg.Any<FoodLog>(), Arg.Any<CancellationToken>());
    }
}
