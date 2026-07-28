# Build context esperado: la raíz del repositorio (docker-compose.yml lo configura así),
# porque NutriMind.API referencia a Domain/Application/Infrastructure como proyectos
# hermanos fuera de esta carpeta.

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia solo los .csproj primero para que la capa de "restore" se cachee entre builds
# y no se invalide cada vez que cambia un .cs sin tocar dependencias.
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
