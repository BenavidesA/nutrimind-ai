using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Application.DTOs.Foods;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodsController : ControllerBase
    {
        private readonly IFoodService _foodService;
        private readonly IFoodLogService _foodLogService;

        // Lista est�tica en memoria para simular la base de datos de forma inmediata y veloz
        private static readonly System.Collections.Generic.List<object> _emergencyLog = new();

        public FoodsController(IFoodService foodService, IFoodLogService foodLogService)
        {
            _foodService = foodService;
            _foodLogService = foodLogService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Search(
            [FromQuery] string? search,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var result = await _foodService.SearchFoodsAsync(search, page, pageSize, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var result = await _foodService.GetFoodByIdAsync(id, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpGet("barcode/{code}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetByBarcode(string code, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(code))
                return BadRequest("El c�digo de barras es requerido.");

            var result = await _foodService.GetFoodByBarcodeAsync(code, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        // ==========================================
        // M�TODOS DE PERSISTENCIA EN MEMORIA VIVA
        // ==========================================

        [HttpGet("log")]
        public async Task<IActionResult> GetUserLog(CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized();

            await Task.Delay(5, cancellationToken);
            return Ok(_emergencyLog);
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddToLog([FromBody] FoodAddRequest request, CancellationToken cancellationToken)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return Unauthorized();

            if (request == null)
                return BadRequest("Datos inv�lidos.");

            var newLogEntry = new
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Calories = request.Calories,
                Carbs = request.Carbs,
                Protein = request.Protein,
                Fat = request.Fat,
                ServingSizeG = request.ServingSizeG,
                LogDate = DateTime.UtcNow
            };

            _emergencyLog.Add(newLogEntry);

            await Task.CompletedTask;
            return Ok(newLogEntry);
        }
    }
}