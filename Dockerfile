# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# This stage is used when running from VS in fast mode (Default for Debug configuration)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 7076
ENV ASPNETCORE_HTTP_PORTS=7076

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

COPY . .
RUN dotnet restore
RUN dotnet build -c $BUILD_CONFIGURATION -o /app/build

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish ./VehicleCategoryManager/VehicleCategoryManager/VehicleCategoryManager.csproj -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Build an EF Core migrations bundle (a standalone executable that applies pending migrations)
RUN dotnet tool install --global dotnet-ef --version 10.0.13
ENV PATH="${PATH}:/root/.dotnet/tools"
ARG ConnectionStrings__DefaultConnection
ENV ConnectionStrings__DefaultConnection=$ConnectionStrings__DefaultConnection
RUN dotnet ef migrations bundle --configuration $BUILD_CONFIGURATION --project /src/VehicleCategoryManager.Persistance/VehicleCategoryManager.Persistance.csproj --startup-project /src/VehicleCategoryManager/VehicleCategoryManager/VehicleCategoryManager.csproj -o /app/efbundle --force

RUN dotnet dev-certs https

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
COPY --from=publish /app/efbundle .
# Apply any pending migrations, then start the app. Uses ConnectionStrings__DefaultConnection from the environment.
ENTRYPOINT ["/bin/sh", "-c", "./efbundle --connection \"$ConnectionStrings__DefaultConnection\" && exec dotnet VehicleCategoryManager.dll"]
