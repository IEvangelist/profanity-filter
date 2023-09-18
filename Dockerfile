FROM mcr.microsoft.com/dotnet/runtime:8.0.0-rc.1-alpine3.18 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.100-rc.1-alpine3.18 AS publish
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet publish ./src/ProfanityFilter.Action/ProfanityFilter.Action.csproj \
  -r linux-musl-x64 \
  -c $BUILD_CONFIGURATION \
  -o /app/publish

# Upgrade musl to remove potential vulnerability
RUN apk upgrade musl

FROM mcr.microsoft.com/dotnet/runtime:8.0.0-rc.1-alpine3.18 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/ProfanityFilter.Action"]