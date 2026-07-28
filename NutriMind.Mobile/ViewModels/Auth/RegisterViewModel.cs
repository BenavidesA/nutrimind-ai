using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.Auth;
using NutriMind.Mobile.Services.Api;
using System;
using System.Threading.Tasks;

namespace NutriMind.Mobile.ViewModels.Auth;

public partial class RegisterViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private bool _isPasswordHidden = true;

    [ObservableProperty]
    private bool _isConfirmPasswordHidden = true;

    public RegisterViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    private void TogglePasswordVisibility() => IsPasswordHidden = !IsPasswordHidden;

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility() => IsConfirmPasswordHidden = !IsConfirmPasswordHidden;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        if (IsBusy) return;

        // Basic validations before hitting the API
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            await Shell.Current.DisplayAlert("Campos Vacíos", "Por favor completa todos los campos requeridos.", "OK");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await Shell.Current.DisplayAlert("Contraseñas", "Las contraseñas ingresadas no coinciden.", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var registerRequest = new RegisterRequest
            {
                Email = Email,
                Password = Password
                // Add additional fields here if your RegisterRequest model requires them (e.g. Username)
            };

            // Actual call to the HTTP service in your ApiService.cs
            var result = await _apiService.RegisterAsync(registerRequest);

            if (result != null)
            {
                await Shell.Current.DisplayAlert("¡Éxito!", "Tu cuenta ha sido creada correctamente.", "OK");
                // Automatically goes back to Login
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo completar el registro. Intenta de nuevo.", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error de Registro", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoBackAsync()
    {
        // Immediately resolves the "I already have an account" redirect
        await Shell.Current.GoToAsync("..");
    }
}