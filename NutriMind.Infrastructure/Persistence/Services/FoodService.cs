using Mapster;
using Microsoft.Extensions.Logging;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.Foods;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Domain.Interfaces.Repositories;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.Infrastructure.Persistence.Services
{
    public class FoodService : IFoodService
    {
        private readonly IFoodRepository _foodRepository;
        private readonly ILogger<FoodService> _logger;

        public FoodService(IFoodRepository foodRepository, ILogger<FoodService> logger)
        {
            _foodRepository = foodRepository;
            _logger = logger;
        }

        public async Task<Result<FoodDto?>> GetFoodByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var food = await _foodRepository.GetByIdAsync(id, cancellationToken);

                if (food == null)
                    return Result<FoodDto?>.Success(null);

                var dto = food.Adapt<FoodDto>();
                return Result<FoodDto?>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el alimento con ID {FoodId}", id);
                return Result<FoodDto?>.Failure("Error interno al obtener el alimento.");
            }
        }

        public async Task<Result<IEnumerable<FoodDto>>> SearchFoodsAsync(
            string? searchTerm,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            try
            {
                var foods = await _foodRepository.SearchFoodsAsync(searchTerm ?? string.Empty, page, pageSize, cancellationToken);
                var dtos = foods.Adapt<IEnumerable<FoodDto>>();
                return Result<IEnumerable<FoodDto>>.Success(dtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar alimentos con término {SearchTerm}", searchTerm);
                return Result<IEnumerable<FoodDto>>.Failure("Error interno al buscar alimentos.");
            }
        }

        public async Task<Result<FoodDto?>> GetFoodByBarcodeAsync(string barcode, CancellationToken cancellationToken = default)
        {
            try
            {
                var food = await _foodRepository.GetByBarcodeAsync(barcode, cancellationToken);

                if (food == null)
                    return Result<FoodDto?>.Success(null);

                var dto = food.Adapt<FoodDto>();
                return Result<FoodDto?>.Success(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al escanear el código de barras {Barcode}", barcode);
                return Result<FoodDto?>.Failure("Error interno al escanear el código de barras.");
            }
        }
    }
}
