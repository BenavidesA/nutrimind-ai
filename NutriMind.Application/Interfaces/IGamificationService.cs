using System;
using System.Threading;
using System.Threading.Tasks;
using NutriMind.Application.Common;
using NutriMind.Domain.Common; // Or the namespace of your responses/Result

namespace NutriMind.Application.Interfaces
{
    public interface IGamificationService
    {
        /// <summary>
        /// Processes the user's streak and points when they log an activity (e.g. logging food).
        /// </summary>
        Task<Result<bool>> ProcessActivityAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks whether the user meets the requirements to earn new badges.
        /// </summary>
        Task<Result<bool>> CheckAndAwardBadgesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}