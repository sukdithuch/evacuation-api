# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 5000
EXPOSE 5001


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Evacuation.API/Evacuation.API.csproj", "Evacuation.API/"]
COPY ["Evacuation.Core/Evacuation.Core.csproj", "Evacuation.Core/"]
COPY ["Evacuation.Domain/Evacuation.Domain.csproj", "Evacuation.Domain/"]
COPY ["Evacuation.Shared/Evacuation.Shared.csproj", "Evacuation.Shared/"]
COPY ["Evacuation.Infrastructure/Evacuation.Infrastructure.csproj", "Evacuation.Infrastructure/"]
RUN dotnet restore "./Evacuation.API/Evacuation.API.csproj"
COPY . .
WORKDIR "/src/Evacuation.API"
RUN dotnet build "./Evacuation.API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Evacuation.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Evacuation.API.dll"]