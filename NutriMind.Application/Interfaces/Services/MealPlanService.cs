#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Mapster;
using Microsoft.Extensions.Logging;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.MealPlans;
using NutriMind.Application.Interfaces;
using NutriMind.Domain.Common;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;

namespace NutriMind.Application.Services;

public class MealPlanService : IMealPlanService
{
    private readonly IMealPlanRepository _mealPlanRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MealPlanService> _logger;

    public MealPlanService(
        IMealPlanRepository mealPlanRepository,
        IUnitOfWork unitOfWork,
        ILogger<MealPlanService> logger)
    {
        _mealPlanRepository = mealPlanRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<MealPlanResponseDto>> CreateMealPlanAsync(Guid userId, CreateMealPlanDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            // Usando Mapster (Regla 12)
            var mealPlan = request.Adapt<MealPlan>();

            // Asignar el usuario actual
            mealPlan.UserId = userId;
            mealPlan.IsAIGenerated = false; // Por defecto no es de IA hasta la Fase 7

            // Forzar fechas a UTC (Regla 9)
            mealPlan.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            mealPlan.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);

            foreach (var plannedMeal in mealPlan.PlannedMeals)
            {
                plannedMeal.Day = DateTime.SpecifyKind(plannedMeal.Day, DateTimeKind.Utc);
            }

            await _mealPlanRepository.AddAsync(mealPlan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = mealPlan.Adapt<MealPlanResponseDto>();
            return Result<MealPlanResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            // Logging (Regla 10)
            _logger.LogError(ex, "Error creating MealPlan for user {UserId}", userId);
            return Result<MealPlanResponseDto>.Failure("Ocurrió un error al crear el plan de alimentación.");
        }
    }

    public async Task<Result<IEnumerable<MealPlanResponseDto>>> GetMealPlansByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var mealPlans = await _mealPlanRepository.GetByUserIdAsync(userId, cancellationToken);
            var responseDtos = mealPlans.Adapt<IEnumerable<MealPlanResponseDto>>();

            return Result<IEnumerable<MealPlanResponseDto>>.Success(responseDtos);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving MealPlans for user {UserId}", userId);
            return Result<IEnumerable<MealPlanResponseDto>>.Failure("Error al obtener los planes de alimentación.");
        }
    }

    public async Task<Result<MealPlanResponseDto>> GetMealPlanByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(id, cancellationToken);

            if (mealPlan == null || mealPlan.UserId != userId)
            {
                return Result<MealPlanResponseDto>.Failure("Plan de alimentación no encontrado o sin permisos.");
            }

            var responseDto = mealPlan.Adapt<MealPlanResponseDto>();
            return Result<MealPlanResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving MealPlan {MealPlanId} for user {UserId}", id, userId);
            return Result<MealPlanResponseDto>.Failure("Error al obtener el plan de alimentación.");
        }
    }

    public async Task<Result<MealPlanResponseDto>> UpdateMealPlanAsync(Guid id, Guid userId, UpdateMealPlanDto request, CancellationToken cancellationToken = default)
    {
        try
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(id, cancellationToken);

            if (mealPlan == null || mealPlan.UserId != userId)
                return Result<MealPlanResponseDto>.Failure("Plan de alimentación no encontrado o sin permisos.");

            mealPlan.Name = request.Name;
            mealPlan.StartDate = DateTime.SpecifyKind(request.StartDate, DateTimeKind.Utc);
            mealPlan.EndDate = DateTime.SpecifyKind(request.EndDate, DateTimeKind.Utc);
            mealPlan.TotalCaloriesPerDay = request.TotalCaloriesPerDay;

            await _mealPlanRepository.UpdateAsync(mealPlan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var responseDto = mealPlan.Adapt<MealPlanResponseDto>();
            return Result<MealPlanResponseDto>.Success(responseDto);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MealPlan {MealPlanId} for user {UserId}", id, userId);
            return Result<MealPlanResponseDto>.Failure("Ocurrió un error al actualizar el plan de alimentación.");
        }
    }

    public async Task<Result<bool>> DeleteMealPlanAsync(Guid id, Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var mealPlan = await _mealPlanRepository.GetByIdAsync(id, cancellationToken);

            if (mealPlan == null || mealPlan.UserId != userId)
            {
                return Result<bool>.Failure("Plan de alimentación no encontrado o sin permisos.");
            }

            // Hard delete por Regla 8 (Solo User tiene Soft Delete)
            await _mealPlanRepository.DeleteAsync(mealPlan, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting MealPlan {MealPlanId} for user {UserId}", id, userId);
            return Result<bool>.Failure("Error al eliminar el plan de alimentación.");
        }
    }
}