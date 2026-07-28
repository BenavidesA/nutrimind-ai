using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NutriMind.Web.Models;
using NutriMind.Web.Services;
using NutriMind.Web.Services.Dtos;

namespace NutriMind.Web.Controllers;

public class AuthController : Controller
{
    private readonly IAuthApiService _authApiService;

    public AuthController(IAuthApiService authApiService)
    {
        _authApiService = authApiService;
    }

    [HttpGet]
    public IActionResult Login()
    {
        // Phase A doesn't have any protected page yet (dashboard, etc.) to redirect to
        // if the user is already authenticated — that check will be added once such a page exists.
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApiService.LoginAsync(new LoginRequestDto
        {
            Email = model.Email,
            Password = model.Password
        });

        if (!result.IsSuccess || result.Data == null)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo iniciar sesión.";
            return View(model);
        }

        await SignInUserAsync(result.Data);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApiService.RegisterAsync(new RegisterRequestDto
        {
            Email = model.Email,
            Password = model.Password,
            FirstName = model.FirstName,
            LastName = model.LastName
        });

        if (!result.IsSuccess || result.Data == null)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo completar el registro.";
            return View(model);
        }

        // RegisterAsync on the backend already returns an AuthResponseDto (automatic login after
        // registering), same behavior the mobile app already has.
        await SignInUserAsync(result.Data);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        // The backend always responds with a generic success whether the account exists or not
        // (anti user-enumeration) — the message here doesn't confirm the email is registered either.
        await _authApiService.ForgotPasswordAsync(new ForgotPasswordRequestDto { Email = model.Email });

        return View("VerifyCode", new VerifyCodeViewModel { Email = model.Email });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApiService.VerifyResetCodeAsync(new VerifyResetCodeRequestDto
        {
            Email = model.Email,
            Code = model.Code
        });

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage ?? "El código es inválido o expiró.";
            return View(model);
        }

        return View("ResetPassword", new ResetPasswordViewModel { Email = model.Email, Code = model.Code });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var result = await _authApiService.ResetPasswordAsync(new ResetPasswordRequestDto
        {
            Email = model.Email,
            Code = model.Code,
            NewPassword = model.NewPassword
        });

        if (!result.IsSuccess)
        {
            model.ErrorMessage = result.ErrorMessage ?? "No se pudo actualizar la contraseña.";
            return View(model);
        }

        TempData["SuccessMessage"] = "Tu contraseña fue actualizada. Ya puedes iniciar sesión.";
        return RedirectToAction("Login");
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        var accessToken = User.FindFirst("access_token")?.Value;
        if (!string.IsNullOrEmpty(accessToken))
        {
            // Revokes the refresh tokens on the server — if it fails (e.g. token already expired),
            // we still close the local session; there's no point leaving the user stuck.
            await _authApiService.LogoutAsync(accessToken);
        }

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }

    private async Task SignInUserAsync(AuthResponseDto auth)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, auth.UserId),
            new(ClaimTypes.Email, auth.Email),
            new(ClaimTypes.GivenName, auth.FirstName),
            new(ClaimTypes.Surname, auth.LastName),
            new(ClaimTypes.Role, auth.Role),
            new("access_token", auth.AccessToken),
            new("refresh_token", auth.RefreshToken)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, new AuthenticationProperties
        {
            IsPersistent = true
        });
    }
}
