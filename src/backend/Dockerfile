# syntax=docker/dockerfile:1.7

FROM mcr.microsoft.com/dotnet/sdk:8.0-alpine AS restore
WORKDIR /src

COPY Api/greenginger.csproj Api/
COPY Application/Application.csproj Application/
COPY Domain/Domain.csproj Domain/
COPY Infrastructure/Infrastructure.csproj Infrastructure/
COPY Persistence/Persistence.csproj Persistence/
RUN dotnet restore Api/greenginger.csproj

FROM restore AS build
COPY . .
RUN dotnet publish Api/greenginger.csproj \
    --configuration Release \
    --output /app/publish \
    /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
RUN apk add --no-cache curl icu-libs tzdata \
    && addgroup -S fabu \
    && adduser -S fabu -G fabu

ENV ASPNETCORE_ENVIRONMENT=Production \
    ASPNETCORE_URLS=http://+:8080 \
    DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false \
    TZ=Asia/Ho_Chi_Minh

WORKDIR /app
COPY --from=build /app/publish .
RUN chown -R fabu:fabu /app

USER fabu
EXPOSE 8080

HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
    CMD curl -fsS http://127.0.0.1:8080/health || exit 1

ENTRYPOINT ["dotnet", "greenginger.dll"]
