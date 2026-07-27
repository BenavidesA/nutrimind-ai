using System.Diagnostics;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Web.Helpers;
using NutriMind.Web.Models;
using NutriMind.Web.Services;

namespace NutriMind.Web.Controllers;

public class HomeController : Controller
{
    private readonly IUserApiService _userApiService;
    private readonly IDashboardApiService _dashboardApiService;

    public HomeController(IUserApiService userApiService, IDashboardApiService dashboardApiService)
    {
        _userApiService = userApiService;
        _dashboardApiService = dashboardApiService;
    }

    // Al tener [Authorize], un visitante sin cookie válida rebota automáticamente a
    // options.LoginPath ("/Auth/Login") configurado en Program.cs, sin lógica manual acá.
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var model = new HomeViewModel
        {
            FirstName = User.FindFirst(ClaimTypes.GivenName)?.Value ?? string.Empty
        };

        var progressResult = await _userApiService.GetProgressAsync();
        if (progressResult.IsSuccess && progressResult.Data != null)
        {
            model.CurrentStreak = progressResult.Data.CurrentStreak;
            model.TotalPoints = progressResult.Data.TotalPoints;
        }

        // Mismo límite de "hoy" que el resto de la app (ver AiController.Chat en la API):
        // se compara por fecha local de Ecuador, no por DateTime.UtcNow.Date crudo.
        var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
        var statsResult = await _dashboardApiService.GetStatsAsync(today, today);
        if (statsResult.IsSuccess)
            model.TodayStats = statsResult.Data;

        var badgesResult = await _userApiService.GetBadgesAsync();
        if (badgesResult.IsSuccess && badgesResult.Data != null)
            model.RecentBadges = badgesResult.Data.Take(3).ToList();

        if (!progressResult.IsSuccess || !statsResult.IsSuccess)
            model.ErrorMessage = progressResult.ErrorMessage ?? statsResult.ErrorMessage;

        return View(model);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
