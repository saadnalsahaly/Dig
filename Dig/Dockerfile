FROM centos:7 AS base

# Ensure we listen on any IP Address 
ENV DOTNET_URLS=http://+:5000

WORKDIR /app
# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER root
WORKDIR /app
EXPOSE 80
EXPOSE 81


# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["Docker/Docker.csproj", "Docker/"]
RUN dotnet restore "./Docker/Docker.csproj"
COPY . .
WORKDIR "/src/Docker"
RUN dotnet build "./Docker.csproj" -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./Docker.csproj" -c $BUILD_CONFIGURATION -o /app/publish #/p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Docker.dll"]
