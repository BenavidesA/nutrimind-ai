using NutriMind.Mobile.ViewModels.Profile;

namespace NutriMind.Mobile.Views.Profile;

public partial class ProfilePage : ContentPage
{
    public ProfilePage(ProfileViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
