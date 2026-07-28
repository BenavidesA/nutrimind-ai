# Expected build context: the repository root (docker-compose.yml configures it that way),
# because NutriMind.API references Domain/Application/Infrastructure as sibling projects
# outside this folder.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy only the .csproj files first so the "restore" layer gets cached between builds
# and isn't invalidated every time a .cs file changes without touching dependencies.
COPY NutriMind.Domain/NutriMind.Domain.csproj NutriMind.Domain/
COPY NutriMind.Application/NutriMind.Application.csproj NutriMind.Application/
COPY NutriMind.Infrastructure/NutriMind.Infrastructure.csproj NutriMind.Infrastructure/
COPY NutriMind.API/NutriMind.API.csproj NutriMind.API/
RUN dotnet restore NutriMind.API/NutriMind.API.csproj

COPY NutriMind.Domain/ NutriMind.Domain/
COPY NutriMind.Application/ NutriMind.Application/
COPY NutriMind.Infrastructure/ NutriMind.Infrastructure/
COPY NutriMind.API/ NutriMind.API/

RUN dotnet publish NutriMind.API/NutriMind.API.csproj \
    -c Release \
    -o /app/publish \
    --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "NutriMind.API.dll"]
