#nullable enable
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.MealPlans;
using NutriMind.Domain.Common; // Adjust if your Result<T> is in a different namespace

namespace NutriMind.Application.Interfaces;

public interface IMealPlanService
{
    Task<Result<MealPlanResponseDto>> CreateMealPlanAsync(Guid userId, CreateMealPlanDto request, CancellationToken cancellationToken = default);

    Task<Result<IEnumerable<MealPlanResponseDto>>> GetMealPlansByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<Result<MealPlanResponseDto>> GetMealPlanByIdAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

    Task<Result<MealPlanResponseDto>> UpdateMealPlanAsync(Guid id, Guid userId, UpdateMealPlanDto request, CancellationToken cancellationToken = default);

    Task<Result<bool>> DeleteMealPlanAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);
}