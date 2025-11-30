#!/bin/bash

# EcommerceApp Docker Management Scripts

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

# Functions
print_info() {
    echo -e "${BLUE}[INFO]${NC} $1"
}

print_success() {
    echo -e "${GREEN}[SUCCESS]${NC} $1"
}

print_warning() {
    echo -e "${YELLOW}[WARNING]${NC} $1"
}

print_error() {
    echo -e "${RED}[ERROR]${NC} $1"
}

# Check if Docker is running
check_docker() {
    if ! docker info >/dev/null 2>&1; then
        print_error "Docker is not running. Please start Docker and try again."
        exit 1
    fi
}

# Build and start development environment
dev() {
    print_info "Starting development environment..."
    check_docker
    docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build
}

# Build and start production environment
prod() {
    print_info "Starting production environment..."
    check_docker
    
    if [ ! -f .env ]; then
        print_warning ".env file not found. Creating from template..."
        cp .env.template .env
        print_warning "Please update .env file with your production values before running again."
        exit 1
    fi
    
    docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
}

# Stop all services
stop() {
    print_info "Stopping all services..."
    docker-compose down
    print_success "All services stopped."
}

# Clean up everything (containers, images, volumes)
clean() {
    print_warning "This will remove all containers, images, and volumes. Are you sure? (y/N)"
    read -r response
    if [[ "$response" =~ ^([yY][eE][sS]|[yY])$ ]]; then
        print_info "Cleaning up..."
        docker-compose down -v --remove-orphans
        docker system prune -a --volumes -f
        print_success "Cleanup completed."
    else
        print_info "Cleanup cancelled."
    fi
}

# Show logs
logs() {
    service=${2:-}
    if [ -n "$service" ]; then
        print_info "Showing logs for $service..."
        docker-compose logs -f "$service"
    else
        print_info "Showing logs for all services..."
        docker-compose logs -f
    fi
}

# Show status
status() {
    print_info "Service status:"
    docker-compose ps
}

# Build specific service
build() {
    service=${2:-}
    if [ -n "$service" ]; then
        print_info "Building $service..."
        docker-compose build "$service"
        print_success "$service built successfully."
    else
        print_info "Building all services..."
        docker-compose build
        print_success "All services built successfully."
    fi
}

# Restart specific service
restart() {
    service=${2:-}
    if [ -n "$service" ]; then
        print_info "Restarting $service..."
        docker-compose restart "$service"
        print_success "$service restarted successfully."
    else
        print_error "Please specify a service name."
        exit 1
    fi
}

# Show help
help() {
    echo "EcommerceApp Docker Management"
    echo ""
    echo "Usage: $0 <command> [options]"
    echo ""
    echo "Commands:"
    echo "  dev                 Start development environment"
    echo "  prod                Start production environment"
    echo "  stop                Stop all services"
    echo "  clean               Clean up all containers, images, and volumes"
    echo "  logs [service]      Show logs (optionally for specific service)"
    echo "  status              Show service status"
    echo "  build [service]     Build services (optionally specific service)"
    echo "  restart <service>   Restart specific service"
    echo "  help                Show this help message"
    echo ""
    echo "Examples:"
    echo "  $0 dev"
    echo "  $0 logs orderservice"
    echo "  $0 build stockservice"
    echo "  $0 restart rabbitmq"
}

# Main script logic
case $1 in
    dev)
        dev
        ;;
    prod)
        prod
        ;;
    stop)
        stop
        ;;
    clean)
        clean
        ;;
    logs)
        logs "$@"
        ;;
    status)
        status
        ;;
    build)
        build "$@"
        ;;
    restart)
        restart "$@"
        ;;
    help|--help|-h)
        help
        ;;
    *)
        print_error "Unknown command: $1"
        echo ""
        help
        exit 1
        ;;
esac