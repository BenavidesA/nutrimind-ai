using System;
using System.Threading;
using System.Threading.Tasks;
using NutriMind.Application.Common;
using NutriMind.Domain.Common; // O el namespace de tus respuestas/Result

namespace NutriMind.Application.Interfaces
{
    public interface IGamificationService
    {
        /// <summary>
        /// Procesa la racha y los puntos del usuario cuando registra una actividad (ej. registrar comida).
        /// </summary>
        Task<Result<bool>> ProcessActivityAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si el usuario cumple los requisitos para ganar nuevas medallas.
        /// </summary>
        Task<Result<bool>> CheckAndAwardBadgesAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}