using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microcharts;
using Microsoft.Extensions.Logging;
using NutriMind.Mobile.Helpers;
using NutriMind.Mobile.Models.Dashboard;
using NutriMind.Mobile.Services.Api;
using SkiaSharp;

namespace NutriMind.Mobile.ViewModels.History;

public partial class HistoryViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly ILogger<HistoryViewModel> _logger;

    // Temporary calorie goal: there is no endpoint yet that exposes the user's actual
    // goal (NutritionGoal.TargetCalories is not exposed to mobile). 2000 kcal/day is the
    // same default value AIAssistantViewModel already uses when generating an AI plan.
    private const decimal DefaultCaloriesGoal = 2000m;

    [ObservableProperty]
    private bool _isBusy;

    [ObservableProperty]
    private int _selectedDaysBack = 7;

    [ObservableProperty]
    private bool _isWeekSelected = true;

    [ObservableProperty]
    private bool _isMonthSelected;

    [ObservableProperty]
    private bool _is3MonthsSelected;

    [ObservableProperty]
    private Chart _caloriesBarChart = BuildEmptyChart();

    [ObservableProperty]
    private Chart _macrosDonutChart = BuildEmptyChart();

    [ObservableProperty] private string _averageCalories = "0 kcal";
    [ObservableProperty] private string _bestCalories = "0 kcal";
    [ObservableProperty] private string _goalDaysAchieved = "0/7 días";
    [ObservableProperty] private string _proteinPercentLabel = "● Prot (0%)";
    [ObservableProperty] private string _carbsPercentLabel = "● Carbs (0%)";
    [ObservableProperty] private string _fatPercentLabel = "● Grasas (0%)";

    public ObservableCollection<DailySummaryDto> DailySummaries { get; } = new();

    public HistoryViewModel(IApiService apiService, ILogger<HistoryViewModel> logger)
    {
        _apiService = apiService;
        _logger = logger;
    }

    [RelayCommand]
    private async Task SelectPeriodAsync(string period)
    {
        SelectedDaysBack = period switch
        {
            "month" => 30,
            "3months" => 90,
            _ => 7
        };
        IsWeekSelected = SelectedDaysBack == 7;
        IsMonthSelected = SelectedDaysBack == 30;
        Is3MonthsSelected = SelectedDaysBack == 90;

        await LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        if (IsBusy) return;
        IsBusy = true;

        try
        {
            // "Today" is calculated in Ecuador time (fixed UTC-5), not raw UTC: between ~7pm and
            // midnight Ecuador time, DateTime.UtcNow.Date had already rolled over to the next
            // day and was leaving out that night's logs.
            var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
            var startDate = today.AddDays(-(SelectedDaysBack - 1));

            var logs = await _apiService.GetFoodLogHistoryAsync(startDate, today);

            var summaries = new List<DailySummaryDto>();
            for (var i = 0; i < SelectedDaysBack; i++)
            {
                var day = startDate.AddDays(i);
                // LogDate comes in raw UTC from the backend — it must be converted to Ecuador
                // time before grouping by day, same as "today" above.
                var dayLogs = logs.Where(l => EcuadorTimeHelper.ToLocal(l.LogDate).Date == day).ToList();

                summaries.Add(new DailySummaryDto
                {
                    Date = DateOnly.FromDateTime(day),
                    TotalCalories = dayLogs.Sum(l => l.Calories),
                    TotalProtein = dayLogs.Sum(l => l.Protein),
                    TotalCarbs = dayLogs.Sum(l => l.Carbs),
                    TotalFat = dayLogs.Sum(l => l.Fat),
                    CaloriesGoal = DefaultCaloriesGoal
                });
            }

            DailySummaries.Clear();
            foreach (var summary in summaries) DailySummaries.Add(summary);

            AverageCalories = $"{summaries.Average(s => s.TotalCalories):N0} kcal";
            BestCalories = $"{summaries.Max(s => s.TotalCalories):N0} kcal";
            var achievedDays = summaries.Count(s => s.GoalAchieved);
            GoalDaysAchieved = $"{achievedDays}/{SelectedDaysBack} días";

            UpdateCharts(summaries);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cargando historial");
        }
        finally
        {
            IsBusy = false;
        }
    }

    private void UpdateCharts(List<DailySummaryDto> summaries)
    {
        // 1. Bar Chart (Calories per day, real data)
        CaloriesBarChart = new BarChart
        {
            Entries = summaries.Select(s => new ChartEntry((float)s.TotalCalories)
            {
                Label = s.DayLabel,
                ValueLabel = $"{Math.Round(s.TotalCalories)}",
                Color = SKColor.Parse("#5E5CE6")
            }).ToArray(),
            LabelTextSize = 30,
            BackgroundColor = SKColors.Transparent
        };

        // 2. Donut Chart (Real macros, summed over the 7 days)
        var totalProtein = summaries.Sum(s => s.TotalProtein);
        var totalCarbs = summaries.Sum(s => s.TotalCarbs);
        var totalFat = summaries.Sum(s => s.TotalFat);
        var totalMacros = totalProtein + totalCarbs + totalFat;

        if (totalMacros <= 0)
        {
            MacrosDonutChart = BuildEmptyChart();
            ProteinPercentLabel = "● Prot (0%)";
            CarbsPercentLabel = "● Carbs (0%)";
            FatPercentLabel = "● Grasas (0%)";
            return;
        }

        var proteinPct = Math.Round(totalProtein / totalMacros * 100);
        var carbsPct = Math.Round(totalCarbs / totalMacros * 100);
        var fatPct = Math.Round(totalFat / totalMacros * 100);

        MacrosDonutChart = new DonutChart
        {
            Entries = new[]
            {
                new ChartEntry((float)totalProtein) { Label = "Prot", ValueLabel = $"{proteinPct}%", Color = SKColor.Parse("#AF52DE") },
                new ChartEntry((float)totalCarbs) { Label = "Carbs", ValueLabel = $"{carbsPct}%", Color = SKColor.Parse("#FF9500") },
                new ChartEntry((float)totalFat) { Label = "Grasas", ValueLabel = $"{fatPct}%", Color = SKColor.Parse("#FF3B30") }
            },
            LabelTextSize = 30,
            BackgroundColor = SKColors.Transparent,
            HoleRadius = 0.6f
        };

        ProteinPercentLabel = $"● Prot ({proteinPct}%)";
        CarbsPercentLabel = $"● Carbs ({carbsPct}%)";
        FatPercentLabel = $"● Grasas ({fatPct}%)";
    }

    private static Chart BuildEmptyChart() => new DonutChart
    {
        Entries = Array.Empty<ChartEntry>(),
        BackgroundColor = SKColors.Transparent,
        HoleRadius = 0.6f
    };
}