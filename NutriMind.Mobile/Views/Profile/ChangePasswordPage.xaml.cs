using NutriMind.Mobile.ViewModels.Profile;

namespace NutriMind.Mobile.Views.Profile;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage(ChangePasswordViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
