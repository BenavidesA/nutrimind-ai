# NutriMindAI

NutriMindAI is a nutrition-tracking platform built around an AI nutrition assistant. Users log the food they eat, get AI-estimated macronutrients for items that aren't in the catalog yet, receive AI-generated meal plans tailored to their calorie and dietary goals, chat with a nutrition assistant, and stay motivated through a gamification system (streaks, points, and badges).

The solution is a single .NET 8 backend API consumed by two independent client applications — a cross-platform mobile app and a web app — both communicating with the backend exclusively over HTTP.

## Key Features

- **AI-powered food logging ("smart add")** — when a logged food isn't already in the catalog, the backend calls the Gemini API once to estimate its macronutrients per 100g, then persists the result in the food catalog so the same food is looked up from the database instead of calling the AI again.
- **AI-generated meal plans** — describes a diet type, target daily calories, and allergies; Gemini returns a structured multi-day plan built exclusively from the real food catalog (to guarantee valid references) and persists it like any manually created plan.
- **AI nutrition chat assistant** — a conversational endpoint backed by Gemini for general nutrition questions and advice.
- **Food logging & history** — manual, quick-add, and smart-add flows for daily food logs, with per-day and date-range history and daily intake summaries.
- **Meal planning** — create, list, and delete multi-day meal plans composed of planned meals per meal type (breakfast, lunch, dinner, snack).
- **Nutrition dashboard** — aggregated stats over a date range (calories, macros, progress) for the authenticated user.
- **Gamification** — daily activity streaks, a points ledger, badges awarded automatically when streak milestones are reached, and a points-based user ranking/leaderboard.
- **User profiles & goals** — age, gender, height, weight, activity level, and dietary goal (lose weight / maintain / gain muscle), used as context for AI recommendations.
- **Authentication** — registration, login, JWT access + refresh tokens, logout, and a full "forgot password" flow with an emailed verification code.
- **Ecuador-aware date handling** — a shared helper converts UTC timestamps to Ecuador's fixed UTC-5 offset (no DST) for anything shown to the user or given to the AI as context, while persisted timestamps stay in UTC.

## Tech Stack

### Architecture

A layered ("Clean Architecture"-inspired) .NET solution with two fully decoupled HTTP clients on top:

```
NutriMind.Domain            (entities, enums, repository interfaces — no external dependencies)
   ^
NutriMind.Application        (service interfaces + implementations, DTOs, validators, Result<T> pattern)
   ^
NutriMind.Infrastructure     (EF Core persistence, JWT auth, email, Gemini AI integration)
   ^
NutriMind.API                (ASP.NET Core minimal-hosting REST API, JWT-secured controllers)

NutriMind.Mobile  --HTTP-->  NutriMind.API   (.NET MAUI, MVVM)
NutriMind.Web     --HTTP-->  NutriMind.API   (ASP.NET Core MVC)
```

- Business logic lives in a classic **service layer** (`I*Service` / `*Service`), not CQRS/MediatR — every service method returns a `Result<T>` instead of throwing for expected business failures.
- Both client apps (`NutriMind.Mobile` and `NutriMind.Web`) are intentionally decoupled from the backend's internal layers: they only reference each other's own DTOs/models and talk to the API purely over HTTP with typed `HttpClient`s.

### Backend

| Layer | Responsibilities | Key libraries |
|---|---|---|
| **Domain** | POCO entities (users, foods, food logs, meal plans, gamification, AI conversations), enums, repository/unit-of-work interfaces | — (no external packages) |
| **Application** | Service layer, DTOs, business validation, object mapping, AI/email integration contracts | FluentValidation, Mapster, Resend |
| **Infrastructure** | EF Core persistence, repositories, JWT issuance, password hashing, Gemini AI client, transactional email | EF Core 8 (SQL Server), BCrypt.Net-Next, System.IdentityModel.Tokens.Jwt |
| **API** | ASP.NET Core minimal-hosting REST API, JWT bearer authentication, Swagger docs | ASP.NET Core 8, JwtBearer, Swashbuckle |

- **Database**: SQL Server via Entity Framework Core (code-first migrations).
- **AI provider**: Google Gemini (`gemini-3.1-flash-lite`) for macro estimation, meal-plan generation, and the chat assistant.
- **Auth**: JWT bearer tokens with refresh tokens; passwords hashed with BCrypt; secrets are read from configuration (.NET user-secrets in development) rather than hardcoded.
- **Transactional email**: Resend, used for password-reset verification codes.

### Mobile (`NutriMind.Mobile`)

.NET MAUI app targeting Android, iOS, Mac Catalyst, and Windows.

- **MVVM** with CommunityToolkit.Mvvm (`[ObservableProperty]` / `[RelayCommand]`) and Shell-based navigation.
- Microcharts.Maui + SkiaSharp for nutrition charts.
- A centralized typed `HttpClient` (`ApiService`) with a `DelegatingHandler` that attaches the stored JWT to outgoing requests.

### Web (`NutriMind.Web`)

ASP.NET Core MVC app (Razor views), just as decoupled from the backend internals as the mobile app.

- Cookie-based session authentication on the web app bridges to the API's JWT: the access/refresh tokens are stored as claims inside an encrypted, HttpOnly session cookie.
- One typed `HttpClient` per API area (auth, food logs, meal plans, AI, dashboard, user), with a delegating handler that forwards the bearer token from the session cookie.

## Getting Started

### Prerequisites

- .NET 8 SDK
- SQL Server (or SQL Server LocalDB) for the API database
- A Google Gemini API key and a Resend API key (only required for the AI and email features)

### Configuration

The API reads its connection string and secrets from configuration rather than source-controlled files. In development, set them with `dotnet user-secrets` from `NutriMind.API`:

```bash
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "<your SQL Server connection string>"
dotnet user-secrets set "JwtSettings:SecretKey" "<a long random secret>"
dotnet user-secrets set "GeminiSettings:ApiKey" "<your Gemini API key>"
dotnet user-secrets set "ResendSettings:ApiKey" "<your Resend API key>"
dotnet user-secrets set "ResendSettings:DevRedirectEmail" "<email to receive dev password-reset codes>"
```

### Running the API

```bash
cd NutriMind.API
dotnet ef database update   # applies EF Core migrations
dotnet run                  # Swagger UI available at /swagger
```

### Running a client

```bash
cd NutriMind.Web
dotnet run                  # update appsettings.json's ApiSettings:BaseUrl if the API runs elsewhere
```

For `NutriMind.Mobile`, open the solution in Visual Studio (or run `dotnet build -t:Run -f net8.0-windows10.0.19041.0`) and select a target platform; the API base URL is defined in `Helpers/Constants.cs`.
