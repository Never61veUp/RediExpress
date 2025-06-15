FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
<<<<<<< HEAD
=======
USER $APP_UID
>>>>>>> origin/master
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
<<<<<<< HEAD

# Copy csproj files
=======
>>>>>>> origin/master
COPY ["RediExpress.AppHost/RediExpress.AppHost.csproj", "RediExpress.AppHost/"]
COPY ["RediExpress.Host/RediExpress.Host.csproj", "RediExpress.Host/"]
COPY ["RediExpress.Application/RediExpress.Application.csproj", "RediExpress.Application/"]
COPY ["RediExpress.Auth/RediExpress.Auth.csproj", "RediExpress.Auth/"]
COPY ["RediExpress.Core/RediExpress.Core.csproj", "RediExpress.Core/"]
COPY ["RediExpress.GeoService/RediExpress.GeoService.csproj", "RediExpress.GeoService/"]
COPY ["RediExpress.EmailService/RediExpress.EmailService.csproj", "RediExpress.EmailService/"]
COPY ["RediExpress.PostgreSql/RediExpress.PostgreSql.csproj", "RediExpress.PostgreSql/"]
COPY ["RediExpress.WeatherService/RediExpress.WeatherService.csproj", "RediExpress.WeatherService/"]
COPY ["RediExpress.Redis/RediExpress.Redis.csproj", "RediExpress.Redis/"]
COPY ["RediExpress.Contracts/RediExpress.Contracts.csproj", "RediExpress.Contracts/"]
COPY ["RediExpress.PostgreSql.MigrationService/RediExpress.PostgreSql.MigrationService.csproj", "RediExpress.PostgreSql.MigrationService/"]
<<<<<<< HEAD

# Restore
RUN dotnet restore "RediExpress.AppHost/RediExpress.AppHost.csproj"

# Copy everything else
COPY . .

# Build
WORKDIR "/src/RediExpress.AppHost"
RUN dotnet build "RediExpress.AppHost.csproj" -c $BUILD_CONFIGURATION -o /src/build

# Publish
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RediExpress.AppHost.csproj" -c $BUILD_CONFIGURATION -o /src/publish /p:UseAppHost=false

# Final image
FROM base AS final
WORKDIR /app
COPY --from=publish /src/publish .
=======
RUN dotnet restore "RediExpress.AppHost/RediExpress.AppHost.csproj"
COPY . .
WORKDIR "/src/RediExpress.AppHost"
RUN dotnet build "RediExpress.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "RediExpress.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
>>>>>>> origin/master
ENTRYPOINT ["dotnet", "RediExpress.AppHost.dll"]
