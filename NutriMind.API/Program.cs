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
using NutriMind.Domain.Entities;
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

// Aplica migraciones de EF Core pendientes al arrancar, para que el contenedor Docker sea
// plug & play sin pasos manuales de "dotnet ef database update". ApplicationDbContext es
// Scoped, así que necesita su propio scope aquí en vez de resolverse desde el
// ServiceProvider raíz. Si la base de datos no está lista o la migración falla, se registra
// el error pero no se tira la app — así el contenedor sigue arriba y el log deja claro qué
// pasó, en vez de crashear en un bucle silencioso.
using (var migrationScope = app.Services.CreateScope())
{
    try
    {
        var dbContext = migrationScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await dbContext.Database.MigrateAsync();
        await SeedDemoDataAsync(dbContext);
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error al aplicar las migraciones o los datos de demo al iniciar la aplicación.");
    }
}

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

await app.RunAsync();

// Reintentos con backoff exponencial (2s, 4s, 8s) ante 429 (saturación de Gemini) y errores
// transitorios (5xx/408/fallas de red de bajo nivel), en vez de fallar el request al primer error.
static IAsyncPolicy<HttpResponseMessage> GetGeminiRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(response => response.StatusCode == HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// Para que el contenedor Docker sea plug & play: sin esto, la base de datos recién migrada
// queda sin ningún usuario y un reclutador que levante "docker-compose up" no tiene con qué
// entrar. "demoUser == null" es el único gate de idempotencia — todo el lote (usuario,
// categoría, alimentos, registros) se siembra una sola vez, en el primer arranque contra una
// base de datos vacía; en arranques siguientes esta función no hace nada.
static async Task SeedDemoDataAsync(ApplicationDbContext dbContext)
{
    const string demoEmail = "demo@nutrimind.com";

    var demoUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Email == demoEmail);
    if (demoUser != null)
        return;

    // Mismo patrón que AuthService.RegisterAsync: no se asigna Id explícitamente, EF Core
    // genera el Guid del usuario al insertar.
    demoUser = new User
    {
        Email = demoEmail,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword("Demo123!"),
        FirstName = "Usuario",
        LastName = "Demo",
        Role = "Student",
        IsActive = true,
        EmailConfirmed = true
    };
    dbContext.Users.Add(demoUser);

    var category = await dbContext.FoodCategories.FirstOrDefaultAsync();
    if (category == null)
    {
        category = new FoodCategory
        {
            Name = "General",
            Description = "Categoría general de alimentos",
            IconUrl = string.Empty
        };
        dbContext.FoodCategories.Add(category);
        // Se guarda ya para obtener el Id autogenerado antes de usarlo como FK en los Food de abajo.
        await dbContext.SaveChangesAsync();
    }

    var chickenBreast = new Food
    {
        Id = Guid.NewGuid(),
        Name = "Pechuga de pollo (cocida)",
        Brand = "Genérico",
        Barcode = string.Empty,
        ServingSizeG = 100,
        ServingUnit = "g",
        CaloriesPer100g = 165,
        ProteinPer100g = 31,
        CarbsPer100g = 0,
        FatPer100g = 3.6m,
        FiberPer100g = 0,
        SugarPer100g = 0,
        SodiumPer100g = 0,
        ImageUrl = string.Empty,
        ExternalId = string.Empty,
        Source = "Seed",
        IsVerified = true,
        FoodCategoryId = category.Id
    };

    var whiteRice = new Food
    {
        Id = Guid.NewGuid(),
        Name = "Arroz blanco (cocido)",
        Brand = "Genérico",
        Barcode = string.Empty,
        ServingSizeG = 100,
        ServingUnit = "g",
        CaloriesPer100g = 130,
        ProteinPer100g = 2.7m,
        CarbsPer100g = 28,
        FatPer100g = 0.3m,
        FiberPer100g = 0,
        SugarPer100g = 0,
        SodiumPer100g = 0,
        ImageUrl = string.Empty,
        ExternalId = string.Empty,
        Source = "Seed",
        IsVerified = true,
        FoodCategoryId = category.Id
    };

    dbContext.Foods.AddRange(chickenBreast, whiteRice);

    var now = DateTime.UtcNow;

    // MealTypeId 1 = Breakfast, 2 = Lunch — sembrados por MealTypeConfiguration.HasData,
    // así que ya existen en cualquier base de datos migrada.
    var breakfastLog = new FoodLog
    {
        Id = Guid.NewGuid(),
        UserId = demoUser.Id,
        FoodId = chickenBreast.Id,
        MealTypeId = 1,
        LogDate = now,
        QuantityG = 150,
        Calories = chickenBreast.CaloriesPer100g * 1.5m,
        Protein = chickenBreast.ProteinPer100g * 1.5m,
        Carbs = chickenBreast.CarbsPer100g * 1.5m,
        Fat = chickenBreast.FatPer100g * 1.5m,
        Notes = string.Empty,
        CreatedAt = now
    };

    var lunchLog = new FoodLog
    {
        Id = Guid.NewGuid(),
        UserId = demoUser.Id,
        FoodId = whiteRice.Id,
        MealTypeId = 2,
        LogDate = now,
        QuantityG = 200,
        Calories = whiteRice.CaloriesPer100g * 2m,
        Protein = whiteRice.ProteinPer100g * 2m,
        Carbs = whiteRice.CarbsPer100g * 2m,
        Fat = whiteRice.FatPer100g * 2m,
        Notes = string.Empty,
        CreatedAt = now
    };

    dbContext.FoodLogs.AddRange(breakfastLog, lunchLog);

    await dbContext.SaveChangesAsync();
}
