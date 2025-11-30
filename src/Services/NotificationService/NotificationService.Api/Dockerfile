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
COPY ["src/Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj", "src/Services/NotificationService/NotificationService.Api/"]
COPY ["src/Contracts/EcommerceApp.Shared/EcommerceApp.Shared.csproj", "src/Contracts/EcommerceApp.Shared/"]

# Restore packages
RUN dotnet restore "src/Services/NotificationService/NotificationService.Api/NotificationService.Api.csproj"

# Copy all source files
COPY . .

# Build the application
WORKDIR "/src/src/Services/NotificationService/NotificationService.Api"
RUN dotnet build "NotificationService.Api.csproj" -c Release -o /app/build

# Publish the application
FROM build AS publish
RUN dotnet publish "NotificationService.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .


ENTRYPOINT ["dotnet", "NotificationService.Api.dll"]