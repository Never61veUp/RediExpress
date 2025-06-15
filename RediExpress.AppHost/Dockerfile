FROM mcr.microsoft.com/dotnet/runtime:9.0 AS base
USER $APP_UID
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
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
RUN dotnet restore "RediExpress.AppHost/RediExpress.AppHost.csproj"
COPY . .
WORKDIR "/src/RediExpress.AppHost"
RUN dotnet build "RediExpress.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN mkdir -p /app/publish && \
    dotnet publish "RediExpress.AppHost.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false && \
    echo "📦 Publish completed" && \
    ls -R /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "RediExpress.AppHost.dll"]
