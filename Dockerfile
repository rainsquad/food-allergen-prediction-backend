# # See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# # This stage is used when running from VS in fast mode (Default for Debug configuration)
# FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
# USER $APP_UID
# WORKDIR /app
# EXPOSE 8080
# EXPOSE 8081


# # This stage is used to build the service project
# FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
# ARG BUILD_CONFIGURATION=Release
# WORKDIR /src
# COPY ["food-allergen-prediction-backend.csproj", "."]
# RUN dotnet restore "./food-allergen-prediction-backend.csproj"
# COPY . .
# WORKDIR "/src/."
# RUN dotnet build "./food-allergen-prediction-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# # This stage is used to publish the service project to be copied to the final stage
# FROM build AS publish
# ARG BUILD_CONFIGURATION=Release
# RUN dotnet publish "./food-allergen-prediction-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# # This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
# FROM base AS final
# WORKDIR /app
# COPY --from=publish /app/publish .
# COPY Models/onnx ./Models/onnx
# ENTRYPOINT ["dotnet", "food-allergen-prediction-backend.dll"]


# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 10000   
# Render dynamically maps $PORT, but exposing 10000 is fine

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["food-allergen-prediction-backend.csproj", "."]
RUN dotnet restore "./food-allergen-prediction-backend.csproj"
COPY . .
RUN dotnet build "./food-allergen-prediction-backend.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./food-allergen-prediction-backend.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY Models/onnx ./Models/onnx

# Start the app
ENTRYPOINT ["dotnet", "food-allergen-prediction-backend.dll"]
