using Microsoft.AspNetCore.Authentication.Cookies;
using NutriMind.Web.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();

// Custom header for the AI chat's fetch() (Views/Ai/Chat.cshtml) — regular forms still
// use @Html.AntiForgeryToken() + the default hidden field, unaffected by this.
builder.Services.AddAntiforgery(options => options.HeaderName = "X-CSRF-TOKEN");

// --- HttpClient to NutriMind.API ---
// 100% decoupled project: only consumes the API over HTTP, doesn't reference
// Domain/Application/Infrastructure (same pattern as NutriMind.Mobile).
var apiBaseUrl = builder.Configuration["ApiSettings:BaseUrl"]
    ?? throw new InvalidOperationException("ApiSettings:BaseUrl no está configurado.");

builder.Services.AddHttpClient<IAuthApiService, AuthApiService>(client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Authenticated endpoints (everything except Auth): same typed HttpClient, but with
// AuthTokenHandler attaching the Bearer token read from the session cookie.
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

// --- Cookie authentication ---
// The JWT's AccessToken/RefreshToken are stored as claims inside this same cookie
// (encrypted via Data Protection, HttpOnly and Secure by default) — no separate cookies are used.
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/Login";
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.Lax;
        // We match the session duration to the RefreshToken's lifetime on the backend (30 days).
        // Silent refresh of the expired AccessToken is left for when there are protected pages
        // that truly need to keep the session alive through repeated calls.
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
