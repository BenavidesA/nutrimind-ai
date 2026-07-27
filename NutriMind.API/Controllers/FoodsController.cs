using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Application.Interfaces.Services;
using System;
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

        public FoodsController(IFoodService foodService)
        {
            _foodService = foodService;
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
    }
}