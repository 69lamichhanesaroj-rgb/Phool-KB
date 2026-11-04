FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["server/API/API.csproj", "server/API/"]
COPY ["server/Service/Service.csproj", "server/Service/"]
COPY ["server/Infra/Infra.csproj", "server/Infra/"]
RUN dotnet restore "server/API/API.csproj"
COPY . .
WORKDIR "/src/server/API"
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish \
    /p:UseAppHost=false --no-restore

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
RUN mkdir -p /app/data && chown -R $APP_UID /app
USER $APP_UID
ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_HTTP_PORTS=8080 \
    DOTNET_EnableDiagnostics=0
HEALTHCHECK --interval=30s --timeout=3s --start-period=20s \
  CMD ["bash", "-c", "exec 3<>/dev/tcp/127.0.0.1/8080"]
ENTRYPOINT ["dotnet", "API.dll"]