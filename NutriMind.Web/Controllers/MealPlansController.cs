using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Web.Models;
using NutriMind.Web.Services;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Controllers;

[Authorize]
public class MealPlansController : Controller
{
    private readonly IMealPlanApiService _mealPlanApiService;
    private readonly IAiApiService _aiApiService;

    public MealPlansController(IMealPlanApiService mealPlanApiService, IAiApiService aiApiService)
    {
        _mealPlanApiService = mealPlanApiService;
        _aiApiService = aiApiService;
    }

    public async Task<IActionResult> Index()
    {
        var result = await _mealPlanApiService.GetAllAsync();
        var model = new MealPlanListViewModel
        {
            Plans = result.Data?.OrderByDescending(p => p.StartDate).ToList() ?? new List<MealPlanResponseDto>(),
            ErrorMessage = result.IsSuccess ? null : result.ErrorMessage
        };
        return View(model);
    }

    public async Task<IActionResult> Details(Guid id)
    {
        var result = await _mealPlanApiService.GetByIdAsync(id);
        if (!result.IsSuccess || result.Data == null)
            return NotFound();

        return View(result.Data);
    }

    [HttpGet]
    public IActionResult Create() => View(new CreateMealPlanViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateMealPlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.EndDate < model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "La fecha de fin debe ser posterior o igual a la fecha de inicio.");
            return View(model);
        }

        var result = await _mealPlanApiService.CreateAsync(new CreateMealPlanDto
        {
            Name = model.Name,
            StartDate = DateTime.SpecifyKind(model.StartDate.Date, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(model.EndDate.Date, DateTimeKind.Utc),
            TotalCaloriesPerDay = model.TotalCaloriesPerDay,
            PlannedMeals = new List<CreatePlannedMealDto>()
        });

        if (!result.IsSuccess || result.Data == null)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo crear el plan de alimentación.";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id)
    {
        var result = await _mealPlanApiService.GetByIdAsync(id);
        if (!result.IsSuccess || result.Data == null)
            return NotFound();

        var plan = result.Data;
        return View(new EditMealPlanViewModel
        {
            Id = plan.Id,
            Name = plan.Name,
            StartDate = plan.StartDate.Date,
            EndDate = plan.EndDate.Date,
            TotalCaloriesPerDay = plan.TotalCaloriesPerDay
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditMealPlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        if (model.EndDate < model.StartDate)
        {
            ModelState.AddModelError(nameof(model.EndDate), "La fecha de fin debe ser posterior o igual a la fecha de inicio.");
            return View(model);
        }

        var result = await _mealPlanApiService.UpdateAsync(model.Id, new UpdateMealPlanDto
        {
            Name = model.Name,
            StartDate = DateTime.SpecifyKind(model.StartDate.Date, DateTimeKind.Utc),
            EndDate = DateTime.SpecifyKind(model.EndDate.Date, DateTimeKind.Utc),
            TotalCaloriesPerDay = model.TotalCaloriesPerDay
        });

        if (!result.IsSuccess || result.Data == null)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo actualizar el plan de alimentación.";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpGet]
    public IActionResult Generate() => View(new GenerateMealPlanViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(GenerateMealPlanViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var allergies = (model.AllergiesText ?? string.Empty)
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .ToList();

        var result = await _aiApiService.GenerateMealPlanAsync(new AiMealPlanRequestDto
        {
            TargetCalories = model.TargetCalories,
            Days = model.Days,
            DietType = string.IsNullOrWhiteSpace(model.DietType) ? "Cualquiera" : model.DietType,
            Allergies = allergies
        });

        if (!result.IsSuccess || result.Data == null)
        {
            model.ErrorMessage = result.ErrorMessage ?? "La IA no pudo generar el plan de alimentación.";
            return View(model);
        }

        return RedirectToAction(nameof(Details), new { id = result.Data.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        await _mealPlanApiService.DeleteAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
