using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Web.Helpers;
using NutriMind.Web.Models;
using NutriMind.Web.Services;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Controllers;

[Authorize]
public class FoodLogController : Controller
{
    private readonly IFoodLogApiService _foodLogApiService;

    public FoodLogController(IFoodLogApiService foodLogApiService)
    {
        _foodLogApiService = foodLogApiService;
    }

    public async Task<IActionResult> Index(DateTime? date)
    {
        var day = (date ?? EcuadorTimeHelper.ToLocal(DateTime.UtcNow)).Date;
        var model = new FoodLogListViewModel { Date = day };

        var result = await _foodLogApiService.GetDailyLogAsync(day);
        if (result.IsSuccess && result.Data != null)
        {
            model.Logs = result.Data;
            model.TotalCalories = result.Data.Sum(l => l.Calories);
            model.TotalProtein = result.Data.Sum(l => l.Protein);
            model.TotalCarbs = result.Data.Sum(l => l.Carbs);
            model.TotalFat = result.Data.Sum(l => l.Fat);
        }
        else
        {
            model.ErrorMessage = result.ErrorMessage;
        }

        return View(model);
    }

    [HttpGet]
    public IActionResult Add(DateTime? date)
    {
        var model = new AddFoodLogViewModel
        {
            Date = (date ?? EcuadorTimeHelper.ToLocal(DateTime.UtcNow)).Date,
            MealTypeId = EcuadorTimeHelper.GetMealTypeIdForNow()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddFoodLogViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _foodLogApiService.SmartAddAsync(new SmartAddFoodLogDto
        {
            Name = model.Name,
            QuantityG = model.QuantityG,
            MealTypeId = model.MealTypeId,
            LogDate = EcuadorTimeHelper.EcuadorDayStartToUtc(model.Date),
            Notes = model.Notes
        });

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo agregar el alimento.";
            return View(model);
        }

        return RedirectToAction(nameof(Index), new { date = model.Date.ToString("yyyy-MM-dd") });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, DateTime date)
    {
        var result = await _foodLogApiService.GetDailyLogAsync(date);
        var log = result.Data?.FirstOrDefault(l => l.Id == id);
        if (log == null)
            return NotFound();

        return View(new EditFoodLogViewModel
        {
            Id = log.Id,
            FoodName = log.FoodName,
            QuantityG = log.QuantityG,
            MealTypeId = log.MealTypeId,
            Date = log.LogDate.Date,
            Notes = log.Notes
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditFoodLogViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _foodLogApiService.UpdateAsync(model.Id, new UpdateFoodLogDto
        {
            MealTypeId = model.MealTypeId,
            LogDate = EcuadorTimeHelper.EcuadorDayStartToUtc(model.Date),
            QuantityG = model.QuantityG,
            Notes = model.Notes
        });

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo actualizar el registro.";
            return View(model);
        }

        return RedirectToAction(nameof(Index), new { date = model.Date.ToString("yyyy-MM-dd") });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, DateTime date)
    {
        await _foodLogApiService.DeleteAsync(id);
        return RedirectToAction(nameof(Index), new { date = date.ToString("yyyy-MM-dd") });
    }
}
