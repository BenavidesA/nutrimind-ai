using Microsoft.AspNetCore.Authentication.Cookies;
using NutriMind.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Header custom para el fetch() del chat de IA (Views/Ai/Chat.cshtml) — los formularios normales
// siguen usando @Html.AntiForgeryToken() + el campo oculto por defecto sin tocar esto.
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

// --- HttpClient hacia NutriMind.API ---
// Proyecto 100% desacoplado: solo consume la API por HTTP, no referencia
// Domain/Application/Infrastructure (mismo patrón que NutriMind.Mobile).
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("ApiSettings:BaseUrl no está configurado.");

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Endpoints autenticados (todo menos Auth): mismo HttpClient tipado, pero con
// AuthTokenHandler adjuntando el Bearer token leído de la cookie de sesión.
builder.Services.AddTransient<AuthTokenHandler>();

builder.Services.AddHttpClient<IFoodLogApiService, FoodLogApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IMealPlanApiService, MealPlanApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IAiApiService, AiApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IDashboardApiService, DashboardApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthTokenHandler>();

builder.Services.AddHttpClient<IUserApiService, UserApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
}).AddHttpMessageHandler<AuthTokenHandler>();

// --- Autenticación por cookie ---
// El AccessToken/RefreshToken del JWT se guardan como claims dentro de esta misma cookie
// (cifrada vía Data Protection, HttpOnly y Secure por defecto) — no se usan cookies separadas.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        // Igualamos la duración de la sesión a la vida del RefreshToken en el backend (30 días).
        // El refresco silencioso del AccessToken vencido queda para cuando existan páginas
        // protegidas que de verdad necesiten mantener la sesión viva con llamadas repetidas.
        options.ExpireTimeSpan = TimeSpan.FromDays(30);
        options.SlidingExpiration = true;
    });

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
