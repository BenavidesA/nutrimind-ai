#nullable enable
using NutriMind.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Domain.Interfaces.Repositories;

public interface IMealPlanRepository
{
    Task<MealPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<IEnumerable<MealPlan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);
    Task UpdateAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);
    Task DeleteAsync(MealPlan mealPlan, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
}