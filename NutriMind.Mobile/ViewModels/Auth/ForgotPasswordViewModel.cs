using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Services.Api;

namespace NutriMind.Mobile.ViewModels.Auth;

public partial class ForgotPasswordViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public ForgotPasswordViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private int currentStep = 1;

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string code = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private async Task SendCodeAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Email))
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa tu correo electrónico.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            await _apiService.ForgotPasswordAsync(Email);
            // The backend always responds with generic success whether or not the account exists
            // (anti-enumeration), so this message doesn't confirm the email is registered either.
            await Shell.Current.DisplayAlert("Listo", "Si el correo está registrado, te enviamos un código de verificación.", "OK");
            CurrentStep = 2;
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task VerifyCodeAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(Code))
        {
            await Shell.Current.DisplayAlert("Error", "Ingresa el código que recibiste.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            var valid = await _apiService.VerifyResetCodeAsync(Email, Code);
            if (valid)
                CurrentStep = 3;
            else
                await Shell.Current.DisplayAlert("Error", "El código es inválido o expiró.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task ResetPasswordAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
        {
            await Shell.Current.DisplayAlert("Error", "La nueva contraseña debe tener al menos 6 caracteres.", "OK");
            return;
        }

        if (NewPassword != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Error", "Las contraseñas no coinciden.", "OK");
            return;
        }

        try
        {
            IsBusy = true;
            var success = await _apiService.ResetPasswordAsync(Email, Code, NewPassword);
            if (success)
            {
                await Shell.Current.DisplayAlert("Listo", "Tu contraseña fue actualizada. Ya puedes iniciar sesión.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar la contraseña. Intenta de nuevo.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Ocurrió un error: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task CancelAsync() => await Shell.Current.GoToAsync("..");
}
