using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Web.Models;
using NutriMind.Web.Services;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Controllers;

[Authorize]
public class ProfileController : Controller
{
    private readonly IUserApiService _userApiService;

    public ProfileController(IUserApiService userApiService)
    {
        _userApiService = userApiService;
    }

    public async Task<IActionResult> Index()
    {
        var profileResult = await _userApiService.GetProfileAsync();
        var badgesResult = await _userApiService.GetBadgesAsync();

        return View(new ProfileIndexViewModel
        {
            Profile = profileResult.Data,
            Badges = badgesResult.Data ?? new List<BadgeResponseDto>(),
            ErrorMessage = profileResult.IsSuccess ? null : profileResult.ErrorMessage
        });
    }

    [HttpGet]
    public async Task<IActionResult> Edit()
    {
        var result = await _userApiService.GetProfileAsync();
        if (!result.IsSuccess || result.Data == null)
            return RedirectToAction(nameof(Index));

        var profile = result.Data;
        return View(new EditProfileViewModel
        {
            FirstName = profile.FirstName,
            LastName = profile.LastName,
            Age = profile.Age,
            Gender = profile.Gender,
            HeightCm = profile.HeightCm,
            WeightKg = profile.WeightKg,
            ActivityLevel = profile.ActivityLevel,
            DietaryGoal = profile.DietaryGoal,
            University = profile.University
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userApiService.UpdateProfileAsync(new UpdateProfileDto
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Age = model.Age,
            Gender = model.Gender,
            HeightCm = model.HeightCm,
            WeightKg = model.WeightKg,
            ActivityLevel = model.ActivityLevel,
            DietaryGoal = model.DietaryGoal,
            University = model.University
        });

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo actualizar tu perfil.";
            return View(model);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult ChangePassword() => View(new ChangePasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _userApiService.ChangePasswordAsync(new ChangePasswordDto
        {
            CurrentPassword = model.CurrentPassword,
            NewPassword = model.NewPassword
        });

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo cambiar la contraseña.";
            return View(model);
        }

        model.SuccessMessage = "Tu contraseña fue actualizada.";
        return View(new ChangePasswordViewModel { SuccessMessage = model.SuccessMessage });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount()
    {
        await _userApiService.DeleteAccountAsync();
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login", "Auth");
    }
}
