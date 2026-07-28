using NutriMind.Mobile.ViewModels.History;

namespace NutriMind.Mobile.Views.History;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;

    // Parameterless constructor (fallback if Shell instantiates without going through DI)
    public HistoryPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(HistoryViewModel)) as HistoryViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor with Dependency Injection (Primary)
    public HistoryPage(HistoryViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = _viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _viewModel?.LoadDataCommand.Execute(null);
    }
}