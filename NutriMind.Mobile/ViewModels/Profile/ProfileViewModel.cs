using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NutriMind.Mobile.Models.Profile;
using NutriMind.Mobile.Services.Api;

namespace NutriMind.Mobile.ViewModels.Profile;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    public ProfileViewModel(IApiService apiService)
    {
        _apiService = apiService;
        _ = LoadProfileAsync();
        _ = LoadBadgesAsync();
    }

    private static readonly Dictionary<ActivityLevel, string> ActivityLevelLabels = new()
    {
        { ActivityLevel.Sedentary, "Sedentario" },
        { ActivityLevel.Light, "Ligero" },
        { ActivityLevel.Moderate, "Moderado" },
        { ActivityLevel.Active, "Activo" },
        { ActivityLevel.VeryActive, "Muy activo" }
    };

    private static readonly Dictionary<string, ActivityLevel> ActivityLevelByLabel =
        ActivityLevelLabels.ToDictionary(kv => kv.Value, kv => kv.Key);

    private static readonly Dictionary<DietaryGoal, string> DietaryGoalLabels = new()
    {
        { DietaryGoal.LoseWeight, "Perder peso" },
        { DietaryGoal.MaintainWeight, "Mantener peso" },
        { DietaryGoal.GainMuscle, "Ganar músculo" }
    };

    private static readonly Dictionary<string, DietaryGoal> DietaryGoalByLabel =
        DietaryGoalLabels.ToDictionary(kv => kv.Value, kv => kv.Key);

    [ObservableProperty] private string firstName = string.Empty;
    [ObservableProperty] private string lastName = string.Empty;
    [ObservableProperty] private string email = string.Empty;
    [ObservableProperty] private string age = string.Empty;
    [ObservableProperty] private string gender = string.Empty;
    [ObservableProperty] private string heightCm = string.Empty;
    [ObservableProperty] private string weightKg = string.Empty;
    [ObservableProperty] private string activityLevelDisplay = ActivityLevelLabels[ActivityLevel.Sedentary];
    [ObservableProperty] private string dietaryGoalDisplay = DietaryGoalLabels[DietaryGoal.MaintainWeight];
    [ObservableProperty] private string university = string.Empty;
    [ObservableProperty] private bool isBusy;

    public List<string> ActivityLevels { get; } = Enum.GetValues<ActivityLevel>().Select(l => ActivityLevelLabels[l]).ToList();
    public List<string> DietaryGoals { get; } = Enum.GetValues<DietaryGoal>().Select(g => DietaryGoalLabels[g]).ToList();

    public ObservableCollection<BadgeResponseDto> Badges { get; } = new();

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;

            var profile = await _apiService.GetProfileAsync();
            if (profile != null)
            {
                FirstName = profile.FirstName;
                LastName = profile.LastName;
                Email = profile.Email;
                Age = profile.Age?.ToString() ?? string.Empty;
                Gender = profile.Gender ?? string.Empty;
                HeightCm = profile.HeightCm?.ToString("0.##") ?? string.Empty;
                WeightKg = profile.WeightKg?.ToString("0.##") ?? string.Empty;
                ActivityLevelDisplay = ActivityLevelLabels[profile.ActivityLevel];
                DietaryGoalDisplay = DietaryGoalLabels[profile.DietaryGoal];
                University = profile.University ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo cargar el perfil: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            await Shell.Current.DisplayAlert("Error", "Nombre y apellido son obligatorios.", "OK");
            return;
        }

        int? ageValue = int.TryParse(Age, out var parsedAge) ? parsedAge : null;
        decimal? heightValue = decimal.TryParse(HeightCm, out var parsedHeight) ? parsedHeight : null;
        decimal? weightValue = decimal.TryParse(WeightKg, out var parsedWeight) ? parsedWeight : null;

        try
        {
            IsBusy = true;

            var dto = new UpdateProfileDto
            {
                FirstName = FirstName,
                LastName = LastName,
                Age = ageValue,
                Gender = Gender,
                HeightCm = heightValue,
                WeightKg = weightValue,
                ActivityLevel = ActivityLevelByLabel.TryGetValue(ActivityLevelDisplay, out var activityLevel) ? activityLevel : ActivityLevel.Sedentary,
                DietaryGoal = DietaryGoalByLabel.TryGetValue(DietaryGoalDisplay, out var dietaryGoal) ? dietaryGoal : DietaryGoal.MaintainWeight,
                University = University
            };

            var success = await _apiService.UpdateProfileAsync(dto);
            if (success)
                await Shell.Current.DisplayAlert("Listo", "Perfil actualizado correctamente.", "OK");
            else
                await Shell.Current.DisplayAlert("Error", "No se pudo actualizar el perfil.", "OK");
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
    private async Task GoBackAsync() => await Shell.Current.GoToAsync("..");

    [RelayCommand]
    private async Task GoToChangePasswordAsync() => await Shell.Current.GoToAsync("changepassword");

    [RelayCommand]
    private async Task LogoutAsync()
    {
        SecureStorage.Default.Remove("jwt_token");
        SecureStorage.Default.Remove("user_id");
        SecureStorage.Default.Remove("user_first_name");
        SecureStorage.Default.Remove("user_last_name");
        await Shell.Current.GoToAsync("//login");
    }

    [RelayCommand]
    private async Task LoadBadgesAsync()
    {
        try
        {
            var badges = await _apiService.GetBadgesAsync();
            Badges.Clear();
            foreach (var badge in badges)
            {
                Badges.Add(badge);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[ProfileViewModel] Error cargando medallas: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task DeleteAccountAsync()
    {
        if (IsBusy) return;

        var confirmar = await Shell.Current.DisplayAlert(
            "Eliminar cuenta",
            "¿Seguro que quieres eliminar tu cuenta? Esta acción no se puede deshacer.",
            "Sí, eliminar", "Cancelar");

        if (!confirmar) return;

        try
        {
            IsBusy = true;

            var success = await _apiService.DeleteAccountAsync();
            if (success)
            {
                SecureStorage.Default.Remove("jwt_token");
                SecureStorage.Default.Remove("user_id");
                SecureStorage.Default.Remove("user_first_name");
                SecureStorage.Default.Remove("user_last_name");
                await Shell.Current.GoToAsync("//login");
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "No se pudo eliminar la cuenta.", "OK");
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
}
