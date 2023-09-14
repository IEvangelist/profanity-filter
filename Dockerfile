#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0.0-rc.1-jammy AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.100-rc.1-jammy AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY . .
RUN dotnet build ./src/ProfanityFilter.Action/ProfanityFilter.Action.csproj -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish ./src/ProfanityFilter.Action/ProfanityFilter.Action.csproj -c $BUILD_CONFIGURATION -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime-deps:8.0.0-rc.1-jammy AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["/app/ProfanityFilter.Action"]