using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Application.DTOs.Foods;
using NutriMind.Application.Interfaces;
using NutriMind.Application.Interfaces.Services;
using System;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace NutriMind.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FoodLogsController : ControllerBase
    {
        private readonly IFoodLogService _foodLogService;
        private readonly IGamificationService _gamificationService;

        public FoodLogsController(IFoodLogService foodLogService, IGamificationService gamificationService)
        {
            _foodLogService = foodLogService;
            _gamificationService = gamificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetDailyLog(
            [FromQuery] DateTime date,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.GetDailyConsumptionAsync(userId, date, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpPost]
        public async Task<IActionResult> Create(
            [FromBody] CreateFoodLogDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.CreateFoodLogAsync(userId, dto, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            await _gamificationService.ProcessActivityAsync(userId, cancellationToken);

            return Ok(result.Data);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] UpdateFoodLogDto dto,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.UpdateFoodLogAsync(userId, id, dto, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(
            Guid id,
            CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.DeleteFoodLogAsync(userId, id, cancellationToken);
            return result.IsSuccess ? NoContent() : BadRequest(result.ErrorMessage);
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistory(
            [FromQuery] DateTime startDate,
            [FromQuery] DateTime endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20,
            CancellationToken cancellationToken = default)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.GetFoodLogHistoryAsync(userId, startDate, endDate, page, pageSize, cancellationToken);
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.ErrorMessage);
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }

        [HttpPost("quick-add")]
        public async Task<IActionResult> QuickAdd(
    [FromBody] QuickAddFoodLogDto dto,
    CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.QuickAddFoodLogAsync(userId, dto, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            await _gamificationService.ProcessActivityAsync(userId, cancellationToken);

            return Ok(result.Data);
        }

        [HttpPost("smart-add")]
        public async Task<IActionResult> SmartAdd(
    [FromBody] SmartAddFoodLogDto dto,
    CancellationToken cancellationToken)
        {
            var userId = GetUserId();
            if (userId == Guid.Empty)
                return Unauthorized();

            var result = await _foodLogService.SmartAddFoodLogAsync(userId, dto, cancellationToken);
            if (!result.IsSuccess)
                return BadRequest(result.ErrorMessage);

            await _gamificationService.ProcessActivityAsync(userId, cancellationToken);

            return Ok(result.Data);
        }
    }


}
