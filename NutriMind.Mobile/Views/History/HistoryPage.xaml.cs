using NutriMind.Mobile.ViewModels.History;

namespace NutriMind.Mobile.Views.History;

public partial class HistoryPage : ContentPage
{
    private readonly HistoryViewModel _viewModel;

    // Constructor sin parámetros (fallback si Shell instancia sin pasar por DI)
    public HistoryPage()
    {
        InitializeComponent();

        var services = Application.Current?.Handler?.MauiContext?.Services;
        _viewModel = (services?.GetService(typeof(HistoryViewModel)) as HistoryViewModel)!;

        BindingContext = _viewModel;
    }

    // Constructor con Inyección de Dependencias (Principal)
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