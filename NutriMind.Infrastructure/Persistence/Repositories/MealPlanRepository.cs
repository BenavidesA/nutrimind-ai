#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NutriMind.Domain.Entities;
using NutriMind.Domain.Interfaces.Repositories;
using NutriMind.Infrastructure.Persistence.Context;

namespace NutriMind.Infrastructure.Persistence.Repositories;

public class MealPlanRepository : IMealPlanRepository
{
    private readonly ApplicationDbContext _context;

    public MealPlanRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MealPlan?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        // Include the planned meals, foods, and meal types
        return await _context.Set<MealPlan>()
            .Include(mp => mp.PlannedMeals)
                .ThenInclude(pm => pm.Food)
            .Include(mp => mp.PlannedMeals)
                .ThenInclude(pm => pm.MealType)
            .FirstOrDefaultAsync(mp => mp.Id == id, cancellationToken);
    }

    public async Task<IEnumerable<MealPlan>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Set<MealPlan>()
            .Where(mp => mp.UserId == userId)
            .OrderByDescending(mp => mp.StartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(MealPlan mealPlan, CancellationToken cancellationToken = default)
    {
        await _context.Set<MealPlan>().AddAsync(mealPlan, cancellationToken);
    }

    public async Task UpdateAsync(MealPlan mealPlan, CancellationToken cancellationToken = default)
    {
        _context.Set<MealPlan>().Update(mealPlan);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(MealPlan mealPlan, CancellationToken cancellationToken = default)
    {
        _context.Set<MealPlan>().Remove(mealPlan);
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Set<MealPlan>().AnyAsync(e => e.Id == id, cancellationToken);
    }
}