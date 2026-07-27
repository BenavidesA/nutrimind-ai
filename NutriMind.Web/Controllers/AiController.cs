using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Web.Services;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Controllers;

[Authorize]
public class AiController : Controller
{
    private readonly IAiApiService _aiApiService;

    public AiController(IAiApiService aiApiService)
    {
        _aiApiService = aiApiService;
    }

    public IActionResult Chat() => View();

    // Endpoint AJAX consumido por el fetch() en Views/Ai/Chat.cshtml — sin persistencia de
    // historial en el servidor (conversación vive solo en el DOM), misma filosofía "sin
    // dependencias extra" que site.js.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChatMessage([FromBody] ChatRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request?.Message))
            return BadRequest(new { error = "El mensaje no puede estar vacío." });

        var result = await _aiApiService.ChatAsync(request.Message);
        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage ?? "El asistente no respondió correctamente." });

        return Json(new { reply = result.Data });
    }
}
