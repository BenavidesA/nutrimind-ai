using NutriMind.Mobile.Views.Food;

namespace NutriMind.Mobile;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("foodlog", typeof(FoodLogPage));
        Routing.RegisterRoute("history", typeof(Views.History.HistoryPage));
        Routing.RegisterRoute("addfood", typeof(Views.Food.AddFoodPage));
        Routing.RegisterRoute("register", typeof(Views.Auth.RegisterPage));
        Routing.RegisterRoute("forgotpassword", typeof(Views.Auth.ForgotPasswordPage));
        Routing.RegisterRoute("changepassword", typeof(Views.Profile.ChangePasswordPage));
        Routing.RegisterRoute("fooddetail", typeof(Views.Food.FoodDetailPage));
        Routing.RegisterRoute("mealplans", typeof(Views.MealPlans.MealPlanPage));
        Routing.RegisterRoute("addmealplan", typeof(Views.MealPlans.AddMealPlanPage));
        Routing.RegisterRoute("profile", typeof(Views.Profile.ProfilePage));
    }
}