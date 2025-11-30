# EcommerceApp - Mikroservis E-Ticaret Uygulamas?

Bu proje, .NET 10 kullan?larak geli?tirilmi? mikroservis mimarisine sahip bir e-ticaret uygulamas?d?r. PostgreSQL veritaban? ve RabbitMQ message broker kullanarak asenkron ileti?im sa?lar.

## ?? ?çindekiler

- [Proje Yap?s?](#-proje-yap?s?)
- [Teknolojiler](#-teknolojiler)
- [H?zl? Ba?lang?ç](#-h?zl?-ba?lang?ç)
- [API Kullan?m?](#-api-kullan?m?)
- [Docker ile Çal??t?rma](#-docker-ile-çal??t?rma)
- [Test Etme](#-test-etme)
- [Mimari](#-mimari)
- [Geli?tirme Notlar?](#-geli?tirme-notlar?)

## ?? Proje Yap?s?

```
EcommerceApp/
??? src/
?   ??? Services/
?   ?   ??? OrderService/           # Sipari? yönetimi servisi
?   ?   ?   ??? OrderService.Api/
?   ?   ??? StockService/           # Stok yönetimi servisi
?   ?   ?   ??? StockService.Api/
?   ?   ??? NotificationService/    # Bildirim servisi
?   ?       ??? NotificationService.Api/
?   ??? Contracts/
?       ??? EcommerceApp.Shared/    # Ortak kütüphaneler ve eventler
??? tests/                          # Test projeleri
??? docker-compose.yml              # Docker Compose konfigürasyonu
??? README.md
```

## ?? Teknolojiler

- **.NET 10** - Ana framework
- **ASP.NET Core Web API** - API geli?tirme
- **PostgreSQL** - Veritaban?
- **Entity Framework Core** - ORM
- **MassTransit** - Message broker integration
- **RabbitMQ** - Message broker
- **MediatR** - CQRS pattern
- **Swagger/OpenAPI** - API dokümantasyonu
- **Docker** - Containerization
- **xUnit** - Unit testing

## ? H?zl? Ba?lang?ç

### Gereksinimler

- .NET 10 SDK
- Docker Desktop
- PostgreSQL (Docker ile çal??acak)
- RabbitMQ (Docker ile çal??acak)

### 1. Projeyi Klonlay?n

```bash
git clone <repository-url>
cd EcommerceApp
```

### 2. Docker ile Çal??t?r?n

```bash
# Development ortam? için
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build

# Veya PowerShell script ile
.\docker-manage.ps1 dev
```

### 3. Servislerin Durumunu Kontrol Edin

```bash
# Health check endpoint'leri
curl http://localhost:5001/health  # OrderService
curl http://localhost:5002/health  # StockService  
curl http://localhost:5003/health  # NotificationService
```

## ?? API Kullan?m?

### ?? Önemli: Kullan?m S?ras?

API'leri kullan?rken **mutlaka** a?a??daki s?ray? takip edin:

1. **Önce stok ekleyin** (ProductInventory)
2. **Sonra sipari? olu?turun** (Order)

### 1. Stok Ekleme (Zorunlu ?lk Ad?m)

Sipari? olu?turmadan önce mutlaka ürün sto?u eklemelisiniz:

```bash
curl -X POST http://localhost:5002/api/inventories/upsert \
  -H "Content-Type: application/json" \
  -d '{
    "productId": 1,
    "quantity": 100
  }'
```

**Aç?klama:** Bu i?lem ProductId=1 olan ürün için 100 adet stok ekler.

### 2. Sipari? Olu?turma

Stok ekledikten sonra sipari? olu?turabilirsiniz:

```bash
curl -X POST http://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 1,
    "items": [
      {
        "productId": 1,
        "productName": "Test Ürünü",
        "quantity": 2,
        "unitPrice": 10.50
      }
    ]
  }'
```

**Aç?klama:** Bu i?lem CustomerId=1 olan mü?teri için 2 adet ProductId=1 ürünü içeren sipari? olu?turur.

### 3. Bildirimleri Görüntüleme

Sipari? i?lemi sonras? otomatik olu?an bildirimleri görün:

```bash
curl http://localhost:5003/api/notifications
```

## ?? Docker ile Çal??t?rma

### Development Ortam?

```bash
# Tüm servisleri ba?lat (PostgreSQL, RabbitMQ, PgAdmin dahil)
docker-compose -f docker-compose.yml -f docker-compose.dev.yml up --build

# Sadece infrastructure servisleri
docker-compose up postgres rabbitmq pgadmin
```

### Production Ortam?

```bash
# Environment dosyas?n? haz?rla
cp .env.template .env
# .env dosyas?n? production de?erleriyle düzenle

# Production modunda ba?lat
docker-compose -f docker-compose.yml -f docker-compose.prod.yml up -d
```

### PowerShell Yönetim Script'i

```powershell
# Development ba?lat
.\docker-manage.ps1 dev

# Production ba?lat  
.\docker-manage.ps1 prod

# Loglar? görüntüle
.\docker-manage.ps1 logs orderservice

# Durumu kontrol et
.\docker-manage.ps1 status

# Temizlik yap
.\docker-manage.ps1 clean
```

## ?? Test Etme

### Tam ?? Ak??? Testi

A?a??daki ad?mlar? s?ras?yla izleyerek tam bir e-ticaret ak???n? test edebilirsiniz:

#### 1. Stok Haz?rl??? (Zorunlu)
```bash
# Ürün 1 için stok ekle
curl -X POST http://localhost:5002/api/inventories/upsert \
  -H "Content-Type: application/json" \
  -d '{"productId": 1, "quantity": 50}'

# Ürün 2 için stok ekle  
curl -X POST http://localhost:5002/api/inventories/upsert \
  -H "Content-Type: application/json" \
  -d '{"productId": 2, "quantity": 30}'
```

#### 2. Ba?ar?l? Sipari? Testi
```bash
# Stokta yeterli ürün olan sipari?
curl -X POST http://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 1,
    "items": [
      {"productId": 1, "productName": "Laptop", "quantity": 1, "unitPrice": 15000.00},
      {"productId": 2, "productName": "Mouse", "quantity": 2, "unitPrice": 250.00}
    ]
  }'
```

#### 3. Stok Yetersizli?i Testi
```bash
# Stokta olmayan/yetersiz ürün sipari?i
curl -X POST http://localhost:5001/api/orders \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": 2,
    "items": [
      {"productId": 1, "productName": "Laptop", "quantity": 100, "unitPrice": 15000.00}
    ]
  }'
```

#### 4. Sonuçlar? Kontrol Etme
```bash
# Bildirimleri kontrol et
curl http://localhost:5003/api/notifications

# RabbitMQ Management Panel'den queue'lar? kontrol et
# http://localhost:15672 (admin/admin123)
```

### Unit Testler

```bash
# Tüm testleri çal??t?r
dotnet test

# Belirli bir proje test et
dotnet test tests/OrderService.Api.Tests/
```

## ?? Mimari

### Mikroservis Yap?s?

- **OrderService**: Sipari? yönetimi, order olu?turma ve durum takibi
- **StockService**: Stok yönetimi, rezervasyon ve güncelleme  
- **NotificationService**: E-posta ve SMS bildirimleri

### Event-Driven ?leti?im

1. **OrderCreated** ? StockService sto?u kontrol eder
2. **OrderStockReserved** ? OrderService + NotificationService bildirilir
3. **StockReservationFailed** ? OrderService sipari? durumunu günceller

### Database Per Service

Her mikroservis kendi PostgreSQL veritaban? ?emas?n? kullan?r:
- OrderService: Order, OrderItem tablolar?
- StockService: ProductInventory tablolar?  
- NotificationService: NotificationMessage tablolar?

## ?? Servisler ve Portlar

| Servis | Port | Aç?klama | Swagger URL |
|--------|------|----------|-------------|
| OrderService | 5001 | Sipari? API'si | http://localhost:5001/swagger |
| StockService | 5002 | Stok API'si | http://localhost:5002/swagger |
| NotificationService | 5003 | Bildirim API'si | http://localhost:5003/swagger |
| PostgreSQL | 5432 | Veritaban? | - |
| RabbitMQ | 5672 | Message Broker | - |
| RabbitMQ Management | 15672 | Yönetim Paneli | http://localhost:15672 |
| PgAdmin (Dev) | 8080 | DB Yönetimi | http://localhost:8080 |

### Yönetim Panel Bilgileri

**RabbitMQ Management:**
- URL: http://localhost:15672
- Kullan?c?: admin
- ?ifre: admin123

**PgAdmin (Development):**
- URL: http://localhost:8080  
- E-posta: admin@ecommerce.com
- ?ifre: admin123

**PostgreSQL Ba?lant?:**
- Host: postgres (Docker içinde) / localhost (lokal)
- Database: EcommerceApp
- Username: postgres
- Password: postgres123

## ?? Geli?tirme Notlar?

### Yeni Mikroservis Ekleme

1. `src/Services/` alt?na yeni klasör olu?turun
2. `Dockerfile` ekleyin
3. `docker-compose.yml` dosyas?na servis tan?m?n? ekleyin
4. Gerekli event'leri `EcommerceApp.Shared` projesine ekleyin

### Event Ekleme

1. `src/Contracts/EcommerceApp.Shared/Contracts/` alt?na event s?n?f? ekleyin
2. ?lgili servislerde consumer/publisher ekleyin
3. MassTransit konfigürasyonunu güncelleyin

### Debugging

```bash
# Belirli servisin loglar?n? takip et
docker-compose logs -f orderservice

# Container'a ba?lan
docker-compose exec orderservice bash

# Veritaban? ba?lant?s?n? test et
docker-compose exec postgres psql -U postgres -d EcommerceApp
```

### Performans ?zleme

- Health check endpoint'leri: `/health`
- Swagger dokümantasyonu: `/swagger`  
- RabbitMQ queue'lar?: Management panel
- Database performans?: PgAdmin

## ?? S?k Kar??la??lan Sorunlar

### "Stok Bulunamad?" Hatas?
**Çözüm:** Sipari? olu?turmadan önce mutlaka `POST /api/inventories/upsert` ile stok ekleyin.

### RabbitMQ Ba?lant? Hatas?  
**Çözüm:** RabbitMQ container'?n?n çal??t???n? kontrol edin: `docker-compose ps`

### Database Ba?lant? Hatas?
**Çözüm:** PostgreSQL container'?n?n haz?r oldu?unu bekleyin. Health check'ler otomatik kontrol eder.

### Port Çak??mas?
**Çözüm:** `docker-compose.yml` dosyas?nda port numaralar?n? de?i?tirin.

## ?? Lisans

Bu proje MIT lisans? alt?nda lisanslanm??t?r.

## ?? Katk?da Bulunma

1. Fork edin
2. Feature branch olu?turun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add some amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)  
5. Pull Request olu?turun

---

**?? Önemli Hat?rlatma:** API'leri test ederken önce stok eklemeyi unutmay?n! Aksi takdirde sipari?leriniz ba?ar?s?z olur.