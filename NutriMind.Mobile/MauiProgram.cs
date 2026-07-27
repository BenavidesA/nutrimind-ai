using CommunityToolkit.Maui;
using NutriMind.Mobile.Services.Api;
using NutriMind.Mobile.Services.Storage;
using NutriMind.Mobile.Helpers;
using NutriMind.Mobile.ViewModels.Auth;
using NutriMind.Mobile.Views.Auth;
using NutriMind.Mobile.ViewModels.AI;
using NutriMind.Mobile.Views.AI;
using NutriMind.Mobile.ViewModels.Food;
using NutriMind.Mobile.Views.Food;

using SkiaSharp.Views.Maui.Controls.Hosting; // <-- AGREGAR ESTO
using Microcharts.Maui;
using UraniumUI;
using NutriMind.Mobile.Views.Home;
using NutriMind.Mobile.ViewModels.Home;                      // <-- AGREGAR ESTO
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

        builder
            .UseMauiApp<App>()
            .UseMauiCommunityToolkit()
            .UseSkiaSharp()
            .UseMicrocharts()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                fonts.AddMaterialSymbolsFonts();
            });

#if ANDROID
        // El framework de Autofill de Android pinta un resaltado amarillo sobre el Entry
        // nativo que Entry.BackgroundColor no puede sobrescribir (se dibuja por debajo del
        // control). Se desactiva a nivel de EditText nativo para todos los Entry de la app.
        // También se limpia el drawable de fondo nativo: cuando BackgroundColor="Transparent",
        // MAUI no siempre reemplaza el underline por defecto del EditText de Android, dejando
        // una línea residual visible sobre fondos custom (ej. las tarjetas grises de inputs).
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


        // Registrar el manejador de autenticación
        builder.Services.AddTransient<AuthHeaderHandler>();

        builder.Services.AddHttpClient<IApiService, ApiService>(client =>
        {
            client.BaseAddress = new Uri(Constants.BaseUrl);
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            var handler = new HttpClientHandler();
#if DEBUG
            // Solo en builds de depuración: acepta el certificado autofirmado del servidor
            // local de desarrollo. Esto nunca debe compilarse en un build de Release/Store.
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
#endif
            return handler;
        })
        .AddHttpMessageHandler<AuthHeaderHandler>();


        builder.Services.AddTransient<AIAssistantViewModel>();
        builder.Services.AddTransient<AIAssistantPage>();

        builder.Services.AddTransient<NutriMind.Mobile.ViewModels.Food.AddFoodViewModel>();
        builder.Services.AddTransient<NutriMind.Mobile.Views.Food.AddFoodPage>();



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