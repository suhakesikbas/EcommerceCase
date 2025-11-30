# EcommerceApp Docker Management PowerShell Script

param(
    [Parameter(Mandatory=$true, Position=0)]
    [string]$Command,
    
    [Parameter(Position=1)]
    [string]$Service = ""
)

# Color functions
function Write-Info {
    param([string]$Message)
    Write-Host "[INFO] $Message" -ForegroundColor Blue
}

function Write-Success {
    param([string]$Message)
    Write-Host "[SUCCESS] $Message" -ForegroundColor Green
}

function Write-Warning {
    param([string]$Message)
    Write-Host "[WARNING] $Message" -ForegroundColor Yellow
}

function Write-Error {
    param([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
}

# Check if Docker is running
function Test-Docker {
    try {
        docker info | Out-Null
        return $true
    }
    catch {
        Write-Error "Docker is not running. Please start Docker and try again."
        return $false
    }
}

# Start development environment
function Start-DevEnvironment {
    Write-Info "Starting development environment..."
    if (-not (Test-Docker)) { exit 1 }
    docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build
}

# Start production environment
function Start-ProdEnvironment {
    Write-Info "Starting production environment..."
    if (-not (Test-Docker)) { exit 1 }
    
    if (-not (Test-Path ".env")) {
        Write-Warning ".env file not found. Creating from template..."
        Copy-Item ".env.template" ".env"
        Write-Warning "Please update .env file with your production values before running again."
        exit 1
    }
    
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
}

# Stop all services
function Stop-Services {
    Write-Info "Stopping all services..."
    docker-compose down
    Write-Success "All services stopped."
}

# Clean up everything
function Remove-Everything {
    $response = Read-Host "This will remove all containers, images, and volumes. Are you sure? (y/N)"
    if ($response -match "^[Yy]([Ee][Ss])?$") {
        Write-Info "Cleaning up..."
        docker-compose down -v --remove-orphans
        docker system prune -a --volumes -f
        Write-Success "Cleanup completed."
    }
    else {
        Write-Info "Cleanup cancelled."
    }
}

# Show logs
function Show-Logs {
    if ($Service) {
        Write-Info "Showing logs for $Service..."
        docker-compose logs -f $Service
    }
    else {
        Write-Info "Showing logs for all services..."
        docker-compose logs -f
    }
}

# Show status
function Show-Status {
    Write-Info "Service status:"
    docker-compose ps
}

# Build services
function Build-Services {
    if ($Service) {
        Write-Info "Building $Service..."
        docker-compose build $Service
        Write-Success "$Service built successfully."
    }
    else {
        Write-Info "Building all services..."
        docker-compose build
        Write-Success "All services built successfully."
    }
}

# Restart service
function Restart-Service {
    if (-not $Service) {
        Write-Error "Please specify a service name."
        exit 1
    }
    
    Write-Info "Restarting $Service..."
    docker-compose restart $Service
    Write-Success "$Service restarted successfully."
}

# Show help
function Show-Help {
    Write-Host "EcommerceApp Docker Management" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Usage: .\docker-manage.ps1 <command> [service]"
    Write-Host ""
    Write-Host "Commands:"
    Write-Host "  dev                 Start development environment"
    Write-Host "  prod                Start production environment" 
    Write-Host "  stop                Stop all services"
    Write-Host "  clean               Clean up all containers, images, and volumes"
    Write-Host "  logs [service]      Show logs (optionally for specific service)"
    Write-Host "  status              Show service status"
    Write-Host "  build [service]     Build services (optionally specific service)"
    Write-Host "  restart <service>   Restart specific service"
    Write-Host "  help                Show this help message"
    Write-Host ""
    Write-Host "Examples:"
    Write-Host "  .\docker-manage.ps1 dev"
    Write-Host "  .\docker-manage.ps1 logs orderservice"
    Write-Host "  .\docker-manage.ps1 build stockservice"
    Write-Host "  .\docker-manage.ps1 restart rabbitmq"
}

# Main script logic
switch ($Command.ToLower()) {
    "dev" { Start-DevEnvironment }
    "prod" { Start-ProdEnvironment }
    "stop" { Stop-Services }
    "clean" { Remove-Everything }
    "logs" { Show-Logs }
    "status" { Show-Status }
    "build" { Build-Services }
    "restart" { Restart-Service }
    "help" { Show-Help }
    default {
        Write-Error "Unknown command: $Command"
        Write-Host ""
        Show-Help
        exit 1
    }
}