#nullable enable
using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using NutriMind.Application.Common;
using NutriMind.Application.DTOs.AI;
using NutriMind.Application.Interfaces;
using NutriMind.Application.Interfaces.Services;
using System.Linq;
namespace NutriMind.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class AiController : ControllerBase
{
    private readonly IAiService _aiService;
    private readonly IGamificationService _gamificationService;
    private readonly IFoodLogService _foodLogService;
    private readonly IMealPlanService _mealPlanService;

    public AiController(
        IAiService aiService,
        IGamificationService gamificationService,
        IFoodLogService foodLogService,
        IMealPlanService mealPlanService)
    {
        _aiService = aiService;
        _gamificationService = gamificationService;
        _foodLogService = foodLogService;
        _mealPlanService = mealPlanService;
    }

    [HttpPost("generate-meal-plan")]
    [EnableRateLimiting("gemini")]
    public async Task<IActionResult> GenerateMealPlan([FromBody] AiMealPlanRequestDto request, CancellationToken cancellationToken)
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        var generated = await _aiService.GenerateMealPlanAsync(request, cancellationToken);
        if (!generated.IsSuccess)
            return BadRequest(generated.ErrorMessage);

        var saved = await _mealPlanService.CreateMealPlanAsync(userId, generated.Data, cancellationToken);
        if (!saved.IsSuccess)
            return BadRequest(saved.ErrorMessage);

        await _gamificationService.ProcessActivityAsync(userId, cancellationToken);

        return Ok(saved.Data);
    }

    [HttpPost("chat")]
    [EnableRateLimiting("gemini")]
    public async Task<IActionResult> Chat([FromBody] ChatRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
            return BadRequest("El mensaje no puede estar vacío.");

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (!Guid.TryParse(userIdClaim, out Guid userId))
            return Unauthorized();

        try
        {
            var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
            var logsResult = await _foodLogService.GetDailyConsumptionAsync(userId, today, cancellationToken);

            string foodsEaten;
            if (logsResult.IsSuccess && logsResult.Data.Any())
            {
                foodsEaten = string.Join("\n", logsResult.Data.Select(f =>
                {
                    var horaLocalEcuador = EcuadorTimeHelper.ToLocal(f.LogDate);
                    return $"- {horaLocalEcuador:HH:mm} {f.FoodName}: {f.Calories} kcal, P: {f.Protein}g, C: {f.Carbs}g, G: {f.Fat}g.";
                }));
            }
            else
            {
                foodsEaten = "El usuario no ha registrado alimentos hoy todavía.";
            }

            var horaActual = EcuadorTimeHelper.ToLocal(DateTime.UtcNow);
            var culturaEc = new CultureInfo("es-EC");
            string promptConContexto = $"Fecha y hora actual: {horaActual.ToString("dddd, dd 'de' MMMM 'de' yyyy, HH:mm", culturaEc)} (Quito, Ecuador)\nContexto actual del usuario hoy:\n{foodsEaten}\n\nPregunta del usuario: {request.Message}";

            var result = await _aiService.GenerateChatResponseAsync(promptConContexto, cancellationToken);
            if (result.IsSuccess)
                // Wrapped in an object on purpose: a "bare" string returned by Ok() falls into
                // StringOutputFormatter (Content-Type: text/plain, no quotes) instead of the
                // JSON formatter, breaking any client that expects JSON (e.g. Web with
                // ReadFromJsonAsync). An object always goes through the JSON formatter.
                return Ok(new ChatResponse { Reply = result.Data });

            return BadRequest(result.ErrorMessage);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno: {ex.Message}");
        }
    }
}

public class ChatRequest
{
    public string Message { get; set; } = string.Empty;
}

public class ChatResponse
{
    public string Reply { get; set; } = string.Empty;
}