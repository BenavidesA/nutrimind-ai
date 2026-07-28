using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using NutriMind.Mobile.Models.Auth;
using NutriMind.Mobile.Services.Api;
using NutriMind.Mobile.Services.Storage;

namespace NutriMind.Mobile.ViewModels.Auth;

public partial class LoginViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly ISecureStorageService _secureStorageService;

    public LoginViewModel(
        IApiService apiService,
        ISecureStorageService secureStorageService)
    {
        _apiService = apiService;
        _secureStorageService = secureStorageService;
    }

    [ObservableProperty]
    private string email = string.Empty;

    [ObservableProperty]
    private string password = string.Empty;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private bool isPasswordPassword = true;

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordPassword = !IsPasswordPassword;
    }

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            ErrorMessage = string.Empty;

            var request = new LoginRequest
            {
                Email = Email,
                Password = Password
            };

            var result = await _apiService.LoginAsync(request);

            if (result is null)
            {
                ErrorMessage = "Error al iniciar sesión. Verifica tus credenciales.";
                return;
            }

            await _secureStorageService.SaveTokenAsync(result.AccessToken);
            await _secureStorageService.SaveUserIdAsync(result.UserId);
            await _secureStorageService.SaveUserNameAsync(result.FirstName, result.LastName);

            // Redirects to Home (removed the incorrect redirect to registration here)
            await Shell.Current.GoToAsync("//home");
        }
        catch (Exception ex)
        {
            ErrorMessage = "Error de conexión: " + ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    // Auto-fills the demo@nutrimind.com credentials (seeded by the API on startup,
    // see Program.cs/SeedDemoDataAsync) and reuses the same LoginAsync flow — this way a
    // recruiter can enter the app without registering or knowing any credentials.
    [RelayCommand]
    private async Task LoginAsDemoAsync()
    {
        if (IsBusy) return;

        Email = "demo@nutrimind.com";
        Password = "Demo123!";

        await LoginAsync();
    }

    // NEW: Dedicated command to navigate to the registration screen
    [RelayCommand]
    private async Task GoToRegisterAsync()
    {
        await Shell.Current.GoToAsync("register");
    }

    [RelayCommand]
    private async Task GoToForgotPasswordAsync()
    {
        await Shell.Current.GoToAsync("forgotpassword");
    }
}