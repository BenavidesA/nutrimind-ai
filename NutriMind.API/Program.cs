using System.Net;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NutriMind.API.Filters;
using NutriMind.Application.Extensions;
using NutriMind.Application.Interfaces;
using NutriMind.Application.Interfaces.Services;
using NutriMind.Application.Services;
using NutriMind.Application.Settings;
using NutriMind.Domain.Interfaces;
using NutriMind.Domain.Interfaces.Repositories;
using NutriMind.Infrastructure.Authentication;
using NutriMind.Infrastructure.Persistence;
using NutriMind.Infrastructure.Persistence.Context;
using NutriMind.Infrastructure.Persistence.Repositories;
using NutriMind.Infrastructure.Persistence.Services;
using Polly;
using Polly.Extensions.Http;
using Resend;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<IFoodRepository, FoodRepository>();
builder.Services.AddScoped<IFoodLogRepository, FoodLogRepository>();
builder.Services.AddScoped<IMealPlanRepository, MealPlanRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFoodService, FoodService>();
builder.Services.AddScoped<IFoodLogService, FoodLogService>();

builder.Services.AddApplicationValidation();
builder.Services.AddApplicationMappings();

// --- CONFIGURACI�N JWT ---
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Misma fuente que AuthService al firmar los tokens (JwtSettings:SecretKey vía
    // configuración/user-secrets) — antes estaba hardcodeada aquí e ignoraba la config,
    // lo que habría roto la validación si algún día se rotaba el valor sin tocar este archivo.
    var secretKey = configuration["JwtSettings:SecretKey"]
        ?? throw new InvalidOperationException("JwtSettings:SecretKey no está configurado.");

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
    };
});

builder.Services.AddControllers(options => options.Filters.Add<ValidationActionFilter>());
builder.Services.AddEndpointsApiExplorer();

// --- CONFIGURACI�N SWAGGER PARA JWT ---
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "NutriMind.API", Version = "v1" });

    // Configuraci�n nativa de HTTP Bearer
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Pega tu token aqu� (NO escribas 'Bearer', solo pega el token crudo)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

builder.Services.AddScoped<IDashboardService, DashboardService>();
// 1. Registrar Settings
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiSettings"));

// 2. Registrar el HttpClient y el Servicio AI
builder.Services.AddHttpClient<IAiService, GeminiAiService>()
    .AddPolicyHandler(GetGeminiRetryPolicy());
builder.Services.AddScoped<IMealPlanService, MealPlanService>();
builder.Services.AddScoped<IGamificationService, GamificationService>();

// --- RESEND (correo transaccional para recuperación de contraseña) ---
builder.Services.Configure<ResendSettings>(builder.Configuration.GetSection("ResendSettings"));
builder.Services.AddResend(builder.Configuration["ResendSettings:ApiKey"]
    ?? throw new InvalidOperationException("ResendSettings:ApiKey no está configurado."));
builder.Services.AddScoped<IEmailService, ResendEmailService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Reintentos con backoff exponencial (2s, 4s, 8s) ante 429 (saturación de Gemini) y errores
// transitorios (5xx/408/fallas de red de bajo nivel), en vez de fallar el request al primer error.
static IAsyncPolicy<HttpResponseMessage> GetGeminiRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}
