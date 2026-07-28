using Mapster;
using Microsoft.Extensions.Logging;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Foods;
using NutriMind.Application.Interfaces;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Infrastructure.Persistence.Services
{
    public class FoodLogService : IFoodLogService
    {
        private readonly IFoodLogRepository _foodLogRepository;
        private readonly IFoodRepository _foodRepository;
        private readonly IRepository<DailyIntakeSummary> _summaryRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAiService _aiService;
        private readonly ILogger<FoodLogService> _logger;

        public FoodLogService(
            IFoodLogRepository foodLogRepository,
            IFoodRepository foodRepository,
            IRepository<DailyIntakeSummary> summaryRepository,
            IUnitOfWork unitOfWork,
            IAiService aiService,
            ILogger<FoodLogService> logger)
        {
            _foodLogRepository = foodLogRepository;
            _foodRepository = foodRepository;
            _summaryRepository = summaryRepository;
            _unitOfWork = unitOfWork;
            _aiService = aiService;
            _logger = logger;
        }

        public async Task<Result<FoodLogResponseDto>> CreateFoodLogAsync(Guid userId, CreateFoodLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var food = await _foodRepository.GetByIdAsync(dto.FoodId, cancellationToken);

                if (food == null)
                    return Result<FoodLogResponseDto>.Failure("El alimento seleccionado no existe.");

                var factor = dto.QuantityG / 100m;
                var foodLog = new FoodLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FoodId = dto.FoodId,
                    MealTypeId = dto.MealTypeId,
                    LogDate = dto.LogDate,
                    QuantityG = dto.QuantityG,
                    Calories = food.CaloriesPer100g * factor,
                    Protein = food.ProteinPer100g * factor,
                    Carbs = food.CarbsPer100g * factor,
                    Fat = food.FatPer100g * factor,
                    Notes = dto.Notes ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                await _foodLogRepository.AddAsync(foodLog, cancellationToken);
                await UpdateDailySummaryAsync(userId, dto.LogDate.Date, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var createdLog = (await _foodLogRepository.GetHistoryForUserAsync(
                    userId, dto.LogDate.Date, dto.LogDate.Date, 1, 1, cancellationToken))
                    .FirstOrDefault(l => l.Id == foodLog.Id);

                if (createdLog == null)
                    return Result<FoodLogResponseDto>.Failure("Error al recuperar el registro creado.");

                var response = createdLog.Adapt<FoodLogResponseDto>();
                return Result<FoodLogResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear el registro de comida para el usuario {UserId}", userId);
                return Result<FoodLogResponseDto>.Failure("Error interno al registrar la comida.");
            }
        }

        public async Task<Result<FoodLogResponseDto>> QuickAddFoodLogAsync(Guid userId, QuickAddFoodLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var food = new Food
                {
                    Id = Guid.NewGuid(),
                    Name = dto.Name,
                    Brand = "Personalizado",
                    Barcode = string.Empty,
                    ServingSizeG = 100,
                    ServingUnit = "g",
                    CaloriesPer100g = dto.Calories,
                    ProteinPer100g = dto.Protein,
                    CarbsPer100g = dto.Carbs,
                    FatPer100g = dto.Fat,
                    FiberPer100g = 0,
                    SugarPer100g = 0,
                    SodiumPer100g = 0,
                    ImageUrl = string.Empty,
                    ExternalId = string.Empty,
                    Source = "UserCreated",
                    IsVerified = false,
                    FoodCategoryId = 1
                };

                await _foodRepository.AddAsync(food, cancellationToken);

                var foodLog = new FoodLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FoodId = food.Id,
                    MealTypeId = dto.MealTypeId,
                    LogDate = dto.LogDate,
                    QuantityG = 100,
                    Calories = food.CaloriesPer100g,
                    Protein = food.ProteinPer100g,
                    Carbs = food.CarbsPer100g,
                    Fat = food.FatPer100g,
                    Notes = dto.Notes ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                await _foodLogRepository.AddAsync(foodLog, cancellationToken);
                await UpdateDailySummaryAsync(userId, foodLog.LogDate.Date, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var createdLog = (await _foodLogRepository.GetHistoryForUserAsync(
                    userId, foodLog.LogDate.Date, foodLog.LogDate.Date, 1, 100, cancellationToken))
                    .FirstOrDefault(l => l.Id == foodLog.Id);

                if (createdLog == null)
                    return Result<FoodLogResponseDto>.Failure("Error al recuperar el registro creado.");

                var response = createdLog.Adapt<FoodLogResponseDto>();
                return Result<FoodLogResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en quick-add para el usuario {UserId}", userId);
                return Result<FoodLogResponseDto>.Failure("Error interno al registrar el alimento.");
            }
        }

        public async Task<Result<FoodLogResponseDto>> SmartAddFoodLogAsync(Guid userId, SmartAddFoodLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var trimmedName = dto.Name.Trim();

                // 1. Check whether the food already exists in the catalog (avoids spending AI tokens)
                var searchResults = await _foodRepository.SearchFoodsAsync(trimmedName, 1, 5, cancellationToken);
                var existingFood = searchResults.FirstOrDefault(f =>
                    string.Equals(f.Name.Trim(), trimmedName, StringComparison.OrdinalIgnoreCase));

                Food food;
                if (existingFood != null)
                {
                    food = existingFood;
                }
                else
                {
                    // 2. It doesn't exist: only here is an AI call spent
                    var estimateResult = await _aiService.EstimateFoodNutritionAsync(trimmedName, cancellationToken);
                    if (!estimateResult.IsSuccess || estimateResult.Data == null)
                        return Result<FoodLogResponseDto>.Failure(estimateResult.ErrorMessage ?? "No se pudo estimar el valor nutricional del alimento.");

                    var estimate = estimateResult.Data;

                    food = new Food
                    {
                        Id = Guid.NewGuid(),
                        Name = trimmedName,
                        Brand = "Estimado por IA",
                        Barcode = string.Empty,
                        ServingSizeG = 100,
                        ServingUnit = "g",
                        CaloriesPer100g = estimate.CaloriesPer100g,
                        ProteinPer100g = estimate.ProteinPer100g,
                        CarbsPer100g = estimate.CarbsPer100g,
                        FatPer100g = estimate.FatPer100g,
                        FiberPer100g = 0,
                        SugarPer100g = 0,
                        SodiumPer100g = 0,
                        ImageUrl = string.Empty,
                        ExternalId = string.Empty,
                        Source = "AiEstimated",
                        IsVerified = false,
                        FoodCategoryId = 1
                    };

                    await _foodRepository.AddAsync(food, cancellationToken);
                }

                // 3. We calculate the actual intake based on the grams provided
                var factor = dto.QuantityG / 100m;
                var foodLog = new FoodLog
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FoodId = food.Id,
                    MealTypeId = dto.MealTypeId,
                    LogDate = dto.LogDate,
                    QuantityG = dto.QuantityG,
                    Calories = food.CaloriesPer100g * factor,
                    Protein = food.ProteinPer100g * factor,
                    Carbs = food.CarbsPer100g * factor,
                    Fat = food.FatPer100g * factor,
                    Notes = dto.Notes ?? string.Empty,
                    CreatedAt = DateTime.UtcNow
                };

                await _foodLogRepository.AddAsync(foodLog, cancellationToken);
                await UpdateDailySummaryAsync(userId, foodLog.LogDate.Date, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var createdLog = (await _foodLogRepository.GetHistoryForUserAsync(
                    userId, foodLog.LogDate.Date, foodLog.LogDate.Date, 1, 100, cancellationToken))
                    .FirstOrDefault(l => l.Id == foodLog.Id);

                if (createdLog == null)
                    return Result<FoodLogResponseDto>.Failure("Error al recuperar el registro creado.");

                var response = createdLog.Adapt<FoodLogResponseDto>();
                return Result<FoodLogResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en smart-add para el usuario {UserId}", userId);
                return Result<FoodLogResponseDto>.Failure("Error interno al registrar el alimento.");
            }
        }

        public async Task<Result<FoodLogResponseDto>> UpdateFoodLogAsync(Guid userId, Guid foodLogId, UpdateFoodLogDto dto, CancellationToken cancellationToken = default)
        {
            try
            {
                var foodLog = await _foodLogRepository.GetByIdAsync(foodLogId, cancellationToken);

                if (foodLog == null || foodLog.UserId != userId)
                    return Result<FoodLogResponseDto>.Failure("Registro no encontrado.");

                var food = await _foodRepository.GetByIdAsync(foodLog.FoodId, cancellationToken);

                if (food == null)
                    return Result<FoodLogResponseDto>.Failure("El alimento asociado ya no existe.");

                var oldDate = foodLog.LogDate.Date;

                var factor = dto.QuantityG / 100m;
                foodLog.MealTypeId = dto.MealTypeId;
                foodLog.LogDate = dto.LogDate;
                foodLog.QuantityG = dto.QuantityG;
                foodLog.Calories = food.CaloriesPer100g * factor;
                foodLog.Protein = food.ProteinPer100g * factor;
                foodLog.Carbs = food.CarbsPer100g * factor;
                foodLog.Fat = food.FatPer100g * factor;
                foodLog.Notes = dto.Notes ?? string.Empty;

                _foodLogRepository.Update(foodLog);

                await UpdateDailySummaryAsync(userId, oldDate, cancellationToken);
                if (dto.LogDate.Date != oldDate)
                    await UpdateDailySummaryAsync(userId, dto.LogDate.Date, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var updatedLog = (await _foodLogRepository.GetHistoryForUserAsync(
                    userId, dto.LogDate.Date, dto.LogDate.Date, 1, int.MaxValue, cancellationToken))
                    .FirstOrDefault(l => l.Id == foodLog.Id);

                if (updatedLog == null)
                    return Result<FoodLogResponseDto>.Failure("Error al recuperar el registro actualizado.");

                var response = updatedLog.Adapt<FoodLogResponseDto>();
                return Result<FoodLogResponseDto>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar el registro de comida {FoodLogId} del usuario {UserId}", foodLogId, userId);
                return Result<FoodLogResponseDto>.Failure("Error interno al actualizar la comida.");
            }
        }

        public async Task<Result<bool>> DeleteFoodLogAsync(Guid userId, Guid foodLogId, CancellationToken cancellationToken = default)
        {
            try
            {
                var foodLog = await _foodLogRepository.GetByIdAsync(foodLogId, cancellationToken);

                if (foodLog == null || foodLog.UserId != userId)
                    return Result<bool>.Failure("Registro no encontrado.");

                _foodLogRepository.Remove(foodLog);
                await UpdateDailySummaryAsync(userId, foodLog.LogDate.Date, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el registro de comida {FoodLogId} del usuario {UserId}", foodLogId, userId);
                return Result<bool>.Failure("Error interno al eliminar la comida.");
            }
        }

        public async Task<Result<IEnumerable<FoodLogResponseDto>>> GetFoodLogHistoryAsync(
            Guid userId, DateTime startDate, DateTime endDate, int page, int pageSize,
            CancellationToken cancellationToken = default)
        {
            try
            {
                // startDate/endDate arrive as calendar days in Ecuador time (the client already
                // computes them that way) — they're converted to the equivalent UTC instant range
                // before comparing against LogDate, which is stored in raw UTC.
                var startUtc = EcuadorTimeHelper.EcuadorDayStartToUtc(startDate);
                var endUtcExclusive = EcuadorTimeHelper.EcuadorDayStartToUtc(endDate).AddDays(1);

                var logs = await _foodLogRepository.GetLogsInUtcRangeAsync(userId, startUtc, endUtcExclusive, page, pageSize, cancellationToken);
                var response = logs.Adapt<IEnumerable<FoodLogResponseDto>>();
                return Result<IEnumerable<FoodLogResponseDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el historial del usuario {UserId}", userId);
                return Result<IEnumerable<FoodLogResponseDto>>.Failure("Error interno al obtener el historial.");
            }
        }

        public async Task<Result<IEnumerable<FoodLogResponseDto>>> GetDailyConsumptionAsync(Guid userId, DateTime date, CancellationToken cancellationToken = default)
        {
            try
            {
                // date arrives as a calendar day in Ecuador time (see comment above).
                var startUtc = EcuadorTimeHelper.EcuadorDayStartToUtc(date);
                var endUtcExclusive = startUtc.AddDays(1);

                var logs = await _foodLogRepository.GetLogsInUtcRangeAsync(userId, startUtc, endUtcExclusive, 1, 100, cancellationToken);
                var response = logs.Adapt<IEnumerable<FoodLogResponseDto>>();
                return Result<IEnumerable<FoodLogResponseDto>>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el consumo diario del usuario {UserId} en {Date}", userId, date.Date);
                return Result<IEnumerable<FoodLogResponseDto>>.Failure("Error interno al obtener el consumo diario.");
            }
        }

        private async Task UpdateDailySummaryAsync(Guid userId, DateTime date, CancellationToken cancellationToken)
        {
            var logsForDate = await _foodLogRepository.GetHistoryForUserAsync(userId, date.Date, date.Date, 1, int.MaxValue, cancellationToken);
            var summary = await _foodLogRepository.GetDailySummaryAsync(userId, date, cancellationToken);
            bool isNew = summary == null;

            if (isNew)
            {
                summary = new DailyIntakeSummary
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    SummaryDate = date.Date,
                    CreatedAt = DateTime.UtcNow
                };
            }

            summary.TotalCalories = logsForDate.Sum(l => l.Calories);
            summary.TotalProtein = logsForDate.Sum(l => l.Protein);
            summary.TotalCarbs = logsForDate.Sum(l => l.Carbs);
            summary.TotalFat = logsForDate.Sum(l => l.Fat);
            summary.UpdatedAt = DateTime.UtcNow;

            if (isNew)
                await _summaryRepository.AddAsync(summary, cancellationToken);
            else
                _summaryRepository.Update(summary);
        }
    }
}