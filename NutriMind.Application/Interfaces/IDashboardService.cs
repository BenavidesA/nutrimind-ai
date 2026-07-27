#nullable enable
using System;
using System.Threading;
using System.Threading.Tasks;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Dashboard;
using NutriMind.Domain.Common;

namespace NutriMind.Application.Interfaces;

public interface IDashboardService
{
    Task<Result<DashboardStatsDto>> GetUserStatsAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}