using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Services.Api;

namespace NutriMind.Mobile.ViewModels.Profile;

public partial class ChangePasswordViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public ChangePasswordViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [ObservableProperty]
    private string currentPassword = string.Empty;

    [ObservableProperty]
    private string newPassword = string.Empty;

    [ObservableProperty]
    private string confirmPassword = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(CurrentPassword) || string.IsNullOrWhiteSpace(NewPassword))
        {
            await Shell.Current.DisplayAlert("Error", "Completa todos los campos.", "OK");
            return;
        }

        if (NewPassword.Length < 6)
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
            var success = await _apiService.ChangePasswordAsync(CurrentPassword, NewPassword);
            if (success)
            {
                await Shell.Current.DisplayAlert("Listo", "Tu contraseña fue actualizada.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo cambiar la contraseña. Verifica tu contraseña actual.", "OK");
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
