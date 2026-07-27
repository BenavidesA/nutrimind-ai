using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using NutriMind.Mobile.Helpers;
using NutriMind.Mobile.Models.Dashboard;
using NutriMind.Mobile.Services.Api;
using NutriMind.Mobile.Services.Storage;
using SkiaSharp;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Globalization;

namespace NutriMind.Mobile.ViewModels.Home;

public partial class HomeViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly ISecureStorageService _secureStorageService;

    public HomeViewModel(IApiService apiService, ISecureStorageService secureStorageService)
    {
        _apiService = apiService;
        _secureStorageService = secureStorageService;
    }

    [ObservableProperty] private int streak;
    [ObservableProperty] private int points;
    [ObservableProperty] private string kcal = "0";
    [ObservableProperty] private string carbs = "0g";
    [ObservableProperty] private string protein = "0g";
    [ObservableProperty] private string fat = "0g";
    [ObservableProperty] private bool isBusy;
    [ObservableProperty] private Chart macrosChart = BuildEmptyMacrosChart();
    [ObservableProperty] private string userInitials = "?";
    [ObservableProperty] private string todayLabel = string.Empty;
    public ObservableCollection<RankingEntryDto> Ranking { get; } = new();

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;

            var formattedDate = DateTime.Now.ToString("dddd, d 'de' MMMM", new CultureInfo("es-EC"));
            TodayLabel = char.ToUpper(formattedDate[0]) + formattedDate.Substring(1);

            var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
            var progressTask = _apiService.GetUserProgressAsync();
            var statsTask = _apiService.GetDashboardStatsAsync(today, today);
            var rankingTask = _apiService.GetRankingAsync(top: 5);
            var currentUserIdTask = _secureStorageService.GetUserIdAsync();
            var userNameTask = _secureStorageService.GetUserNameAsync();

            await Task.WhenAll(progressTask, statsTask, rankingTask, currentUserIdTask, userNameTask);

            var progress = progressTask.Result;
            if (progress != null)
            {
                Streak = progress.CurrentStreak;
                Points = progress.TotalPoints;
            }

            var stats = statsTask.Result;
            if (stats != null)
            {
                Kcal = Math.Round(stats.TotalCaloriesConsumed).ToString();
                Carbs = $"{Math.Round(stats.TotalCarbs)}g";
                Protein = $"{Math.Round(stats.TotalProtein)}g";
                Fat = $"{Math.Round(stats.TotalFat)}g";
                MacrosChart = BuildMacrosChart(stats.TotalCarbs, stats.TotalProtein, stats.TotalFat);
            }

            var currentUserId = currentUserIdTask.Result;
            Ranking.Clear();
            foreach (var entry in rankingTask.Result)
            {
                entry.IsCurrentUser = !string.IsNullOrEmpty(currentUserId)
                    && string.Equals(entry.UserId, currentUserId, StringComparison.OrdinalIgnoreCase);
                Ranking.Add(entry);
            }

            var (firstName, lastName) = userNameTask.Result;
            UserInitials = ComputeInitials(firstName, lastName);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[HomeViewModel] Error cargando datos: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
    [RelayCommand]
    private async Task GoToFoodLogAsync() => await Shell.Current.GoToAsync("foodlog");

    [RelayCommand]
    private async Task GoToAIAssistantAsync() => await Shell.Current.GoToAsync("///aiassistant");

    [RelayCommand]
    private async Task GoToHistoryAsync() => await Shell.Current.GoToAsync("history");

    [RelayCommand]
    private async Task GoToMealPlansAsync() => await Shell.Current.GoToAsync("mealplans");

    [RelayCommand]
    private async Task GoToProfileAsync() => await Shell.Current.GoToAsync("profile");

    private static string ComputeInitials(string? firstName, string? lastName)
    {
        var first = string.IsNullOrWhiteSpace(firstName) ? string.Empty : firstName.Trim()[0].ToString();
        var last = string.IsNullOrWhiteSpace(lastName) ? string.Empty : lastName.Trim()[0].ToString();
        var initials = (first + last).ToUpperInvariant();
        return string.IsNullOrEmpty(initials) ? "?" : initials;
    }

    private static Chart BuildEmptyMacrosChart() => new DonutChart
    {
        Entries = Array.Empty<ChartEntry>(),
        BackgroundColor = SKColors.Transparent,
        HoleRadius = 0.6f,
        LabelMode = LabelMode.None
    };

    private static Chart BuildMacrosChart(decimal carbs, decimal protein, decimal fat)
    {
        if (carbs + protein + fat <= 0)
            return BuildEmptyMacrosChart();

        return new DonutChart
        {
            Entries = new[]
            {
                new ChartEntry((float)carbs) { Label = "Carbs", ValueLabel = $"{Math.Round(carbs)}g", Color = SKColor.Parse("#FF9500"), TextColor = SKColor.Parse("#FF9500"), ValueLabelColor = SKColor.Parse("#FF9500") },
                new ChartEntry((float)protein) { Label = "Prot", ValueLabel = $"{Math.Round(protein)}g", Color = SKColor.Parse("#AF52DE"), TextColor = SKColor.Parse("#AF52DE"), ValueLabelColor = SKColor.Parse("#AF52DE") },
                new ChartEntry((float)fat) { Label = "Grasas", ValueLabel = $"{Math.Round(fat)}g", Color = SKColor.Parse("#FF3B30"), TextColor = SKColor.Parse("#FF3B30"), ValueLabelColor = SKColor.Parse("#FF3B30") }
            },
            BackgroundColor = SKColors.Transparent,
            HoleRadius = 0.6f,
            LabelTextSize = 30,
            // Se desactivan las captions nativas (dibujaban una leyenda izquierda/derecha
            // desalineada) — la leyenda real la arma HomePage.xaml a mano, mismo patrón que
            // HistoryPage.
            LabelMode = LabelMode.None
        };
    }
}