# E-Commerce Microservices Application

A microservices-based e-commerce application built with .NET 10.

## Services

- **Order Service**: Manages customer orders (Ports: 5001-5002)
- **Stock Service**: Handles product inventory (Ports: 5003-5004)
- **Notification Service**: Sends notifications (Ports: 5005-5006)

## Quick Start

### Prerequisites
- Docker & Docker Compose
- .NET 10 SDK (for development)

### Running with Docker Compose

```bash
# Build and start all services
docker-compose up --build

# Start in background
docker-compose up -d

# Stop services
docker-compose down

# View logs
docker-compose logs -f
```

### Service URLs

- Order Service: http://localhost:5001
- Stock Service: http://localhost:5003  
- Notification Service: http://localhost:5005

## Development

### Build Solution
```bash
dotnet build EcommerceApp.sln
```

### Run Tests
```bash
dotnet test
```

### Individual Service Development
Navigate to each service directory and run:
```bash
dotnet run
```

## Project Structure

```
??? src/
?   ??? Services/
?   ?   ??? OrderService/
?   ?   ??? StockService/
?   ?   ??? NotificationService/
?   ??? Contracts/
?       ??? EcommerceApp.Shared/
??? tests/
??? docker-compose.yml
??? README.md
```