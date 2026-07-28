using Microsoft.Extensions.Logging;
using NutriMind.Mobile.Services.Api;
using NutriMind.Mobile.Services.Storage;
using NutriMind.Mobile.Helpers;
using NutriMind.Mobile.ViewModels.Auth;
using NutriMind.Mobile.Views.Auth;
using NutriMind.Mobile.ViewModels.AI;
using NutriMind.Mobile.Views.AI;
using NutriMind.Mobile.ViewModels.Food;
using NutriMind.Mobile.Views.Food;

using SkiaSharp.Views.Maui.Controls.Hosting;
using Microcharts.Maui;
using NutriMind.Mobile.Views.Home;
using NutriMind.Mobile.ViewModels.Home;
using NutriMind.Mobile.ViewModels.MealPlans;
using NutriMind.Mobile.Views.MealPlans;
using NutriMind.Mobile.ViewModels.Profile;
using NutriMind.Mobile.Views.Profile;
using NutriMind.Mobile.ViewModels.History;
using NutriMind.Mobile.Views.History;

namespace NutriMind.Mobile;


public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder.Logging.AddDebug();

        builder
            .UseMauiApp<App>()
            .UseSkiaSharp()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                // Alias used as FontFamily="MaterialOutlined" in LoginPage/RegisterPage/
                // HomePage/etc. for icon glyphs (email, lock, eye...). The
                // file itself (MaterialIconsOutlined-Regular.ttf) does not yet exist in
                // Resources/Fonts — without it, MAUI falls back to a replacement font that maps
                // those same Private Use Area codepoints to CJK characters, which is why they
                // show up as Chinese instead of icons.
                fonts.AddFont("MaterialIconsOutlined-Regular.ttf", "MaterialOutlined");
            });

#if ANDROID
        // The Android Autofill framework paints a yellow highlight over the native Entry
        // that Entry.BackgroundColor cannot override (it is drawn underneath the
        // control). It is disabled at the native EditText level for every Entry in the app.
        // The native background drawable is also cleared: when BackgroundColor="Transparent",
        // MAUI does not always replace Android's default EditText underline, leaving
        // a residual line visible over custom backgrounds (e.g. the gray input cards).
        Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("DisableAutofillHighlight", (handler, view) =>
        {
            handler.PlatformView.ImportantForAutofill = Android.Views.ImportantForAutofill.NoExcludeDescendants;
            handler.PlatformView.Background = null;
        });
#endif

        // =========================
        // STORAGE
        // =========================
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();

        // =========================
        // PAGES + VIEWMODELS
        // =========================
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<LoginViewModel>();


        // Register the authentication handler
        builder.Services.AddTransient<AuthHeaderHandler>();

        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri(Constants.BaseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
#if DEBUG
            // Debug builds only: accepts the self-signed certificate from the
            // local development server. This must never ship in a Release/Store build.
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
            return handler;
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();


        builder.Services.AddTransient<AIAssistantViewModel>();
        builder.Services.AddTransient<AIAssistantPage>();

        builder.Services.AddTransient<FoodLogViewModel>();
        builder.Services.AddTransient<FoodLogPage>();
        builder.Services.AddTransient<AddFoodViewModel>();
        builder.Services.AddTransient<AddFoodPage>();
        builder.Services.AddTransient<FoodDetailViewModel>();
        builder.Services.AddTransient<FoodDetailPage>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ForgotPasswordViewModel>();
        builder.Services.AddTransient<ForgotPasswordPage>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<HomePage>();

        builder.Services.AddTransient<MealPlanViewModel>();
        builder.Services.AddTransient<MealPlanPage>();
        builder.Services.AddTransient<AddMealPlanViewModel>();
        builder.Services.AddTransient<AddMealPlanPage>();

        builder.Services.AddTransient<ProfileViewModel>();
        builder.Services.AddTransient<ProfilePage>();
        builder.Services.AddTransient<ChangePasswordViewModel>();
        builder.Services.AddTransient<ChangePasswordPage>();

        builder.Services.AddTransient<HistoryViewModel>();
        builder.Services.AddTransient<HistoryPage>();
        return builder.Build();


    }
}