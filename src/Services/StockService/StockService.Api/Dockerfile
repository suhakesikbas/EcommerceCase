# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Use the official .NET SDK image for building
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution file
COPY ["EcommerceApp.sln", "."]

# Copy project files
COPY ["src/Services/StockService/StockService.Api/StockService.Api.csproj", "src/Services/StockService/StockService.Api/"]
COPY ["src/Contracts/EcommerceApp.Shared/EcommerceApp.Shared.csproj", "src/Contracts/EcommerceApp.Shared/"]

# Restore packages
RUN dotnet restore "src/Services/StockService/StockService.Api/StockService.Api.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/Services/StockService/StockService.Api"
RUN dotnet build "StockService.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "StockService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

ENTRYPOINT ["dotnet", "StockService.Api.dll"]