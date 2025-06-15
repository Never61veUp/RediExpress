FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

COPY RediExpress.AppHost ./RediExpress.AppHost

RUN dotnet restore ./RediExpress.AppHost
RUN dotnet publish ./RediExpress.AppHost -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "RediExpress.AppHost.dll"]
