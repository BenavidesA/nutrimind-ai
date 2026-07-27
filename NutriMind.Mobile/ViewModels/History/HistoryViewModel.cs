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

    // Meta calórica temporal: todavía no existe un endpoint que exponga la meta real del
    // usuario (NutritionGoal.TargetCalories no está expuesto al móvil). 2000 kcal/día es el
    // mismo valor por defecto que ya usa AIAssistantViewModel al generar un plan de IA.
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
            // "Hoy" se calcula en hora Ecuador (UTC-5 fijo), no en UTC crudo: entre ~7pm y
            // medianoche hora Ecuador, DateTime.UtcNow.Date ya cayó en el día siguiente y
            // dejaba fuera los registros de esa noche.
            var today = EcuadorTimeHelper.ToLocal(DateTime.UtcNow).Date;
            var startDate = today.AddDays(-(SelectedDaysBack - 1));

            var logs = await _apiService.GetFoodLogHistoryAsync(startDate, today);

            var summaries = new List<DailySummaryDto>();
            for (var i = 0; i < SelectedDaysBack; i++)
            {
                var day = startDate.AddDays(i);
                // LogDate viene en UTC crudo desde el backend — hay que convertirlo a hora
                // Ecuador antes de agrupar por día, igual que "today" arriba.
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
        // 1. Gráfico de Barras (Calorías por día, datos reales)
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

        // 2. Gráfico de Donut (Macros reales, sumados sobre los 7 días)
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