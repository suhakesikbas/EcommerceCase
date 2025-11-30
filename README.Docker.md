# EcommerceApp Docker Setup

Bu proje Docker ve Docker Compose kullanarak mikroservis mimarisini destekler.

## Gereksinimler

- Docker Engine 20.10.0+
- Docker Compose 2.0.0+

## H?zl? Ba?lang?ç

### Development Ortam?nda Çal??t?rma

```bash
# Tüm servisleri development modunda ba?lat
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build

# Sadece infrastructure servislerini ba?lat (PostgreSQL, RabbitMQ, PgAdmin)
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up postgres rabbitmq pgadmin
```

### Production Ortam?nda Çal??t?rma

```bash
# Environment dosyas?n? kopyala ve düzenle
cp .env.template .env
# .env dosyas?ndaki de?erleri production de?erleriyle güncelle

# Production modunda ba?lat
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

## Servisler ve Portlar

| Servis | Development Port | Production Port | Aç?klama |
|--------|------------------|-----------------|----------|
| Order Service | 5001 | 5001 | Sipari? yönetimi API'si |
| Stock Service | 5002 | 5002 | Stok yönetimi API'si |
| Notification Service | 5003 | 5003 | Bildirim servisi API'si |
| PostgreSQL | 5432 | 5432 | Veritaban? |
| RabbitMQ | 5672 | 5672 | Message Broker |
| RabbitMQ Management | 15672 | 15672 | RabbitMQ Yönetim Paneli |
| PgAdmin (Dev only) | 8080 | - | PostgreSQL Yönetim Paneli |

## API Endpoints

### Development Ortam?nda Test

```bash
# Order Service Health Check
curl http://localhost:5001/health

# Create Order
curl -X POST http://localhost:5001/api/orders \\
  -H "Content-Type: application/json" \\
  -d '{
    "customerId": 1,
    "items": [
      {
        "productId": 1,
        "productName": "Test Product",
        "quantity": 2,
        "unitPrice": 10.00
      }
    ]
  }'

# Get Notifications
curl http://localhost:5003/api/notifications

# Upsert Inventory
curl -X POST http://localhost:5002/api/inventories/upsert \\
  -H "Content-Type: application/json" \\
  -d '{
    "productId": 1,
    "quantity": 100
  }'
```

## Veritaban? Yönetimi

### PgAdmin (Development)
- URL: http://localhost:8080
- Email: admin@ecommerce.com
- Password: admin123

### Connection Details:
- Host: postgres
- Database: EcommerceApp
- Username: postgres
- Password: postgres123

## RabbitMQ Yönetimi

- Management UI: http://localhost:15672
- Username: admin
- Password: admin123

## Docker Commands

```bash
# Tüm servisleri durdur
docker-compose down

# Volumes dahil her ?eyi temizle
docker-compose down -v --remove-orphans

# Sadece belirli bir servisi rebuild et
docker-compose build orderservice

# Loglar? görüntüle
docker-compose logs -f orderservice

# Container'a ba?lan
docker-compose exec orderservice bash
```

## Troubleshooting

### Database Connection Issues
1. PostgreSQL container'?n?n çal??t???n? kontrol edin
2. Connection string'i do?rulay?n
3. Network connectivity'sini kontrol edin

### RabbitMQ Connection Issues  
1. RabbitMQ container'?n?n çal??t???n? kontrol edin
2. RabbitMQ credentials'?n? do?rulay?n
3. Queue'lar?n düzgün olu?turuldu?unu kontrol edin

### Build Issues
1. Docker cache'i temizleyin: `docker system prune`
2. .dockerignore dosyas?n? kontrol edin
3. Build context'ini do?rulay?n

## Environment Variables

### Development
Tüm gerekli environment variable'lar docker-compose.yml dosyas?nda tan?ml?d?r.

### Production
`.env.template` dosyas?n? `.env` olarak kopyalay?n ve production de?erleriyle güncelleyin.

## Security Best Practices

1. Production ortam?nda default ?ifreleri de?i?tirin
2. Database ve RabbitMQ için güçlü ?ifreler kullan?n
3. Environment variables'lar? güvenli bir ?ekilde saklay?n
4. Network access'i production ortam?nda s?n?rland?r?n