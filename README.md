# 🎬 FilmDukkani – Film Kiralama ve Yönetim Sistemi

FilmDukkani, abonelik tabanlı film kiralama mantığıyla çalışan; modern Onion Architecture mimarisi ve SOLID prensipleri esas alınarak geliştirilmiş bir .NET 8 web uygulamasıdır.

Proje, klasik film kiralama senaryolarını temel alır ancak günümüz yazılım standartlarına uygun şekilde ölçeklenebilir, bakımı kolay ve katmanlı bir mimari ile inşa edilmiştir.

---

## 🚀 Kullanılan Teknolojiler

- .NET 8
- ASP.NET Core Web API
- ASP.NET Core MVC
- Entity Framework Core
- MS SQL Server
- JWT Authentication
- Onion Architecture
- SOLID Principles
- Repository & Service Pattern

---

## 🧅 Mimari Yaklaşım – Onion Architecture

Proje Onion Architecture yaklaşımı ile geliştirilmiştir.

- Domain katmanı dış katmanlardan tamamen bağımsızdır
- Application katmanı iş kuralları ve senaryoları içerir
- Infrastructure katmanı teknik detaylardan sorumludur
- API ve MVC katmanları yalnızca sunum (presentation) amaçlıdır

Bağımlılıklar her zaman dıştan içe doğru akar.

---

## 📂 Proje Klasör Yapısı

```text
FilmDukkani
│
├── Domain
│   ├── Entities
│   ├── Enums
│   └── Core domain rules
│
├── Application
│   ├── DTOs
│   │   ├── AccountingDTOs
│   │   ├── MovieDTOs
│   │   ├── MemberDTOs
│   │   └── DeliveryRequestDTOs
│   │
│   ├── Repositories
│   │   └── Interfaces
│   │
│   ├── ServiceManager
│   │   ├── MovieServiceManager
│   │   ├── DeliveryRequestServiceManager
│   │   └── AccountingServiceManager
│   │
│   └── Constants
│
├── Infrastructure
│   ├── Persistence
│   │   ├── FilmDukkaniDbContext
│   │   └── EntityConfigurations
│   │
│   ├── Repositories
│   │   ├── MovieRepository
│   │   ├── MemberRepository
│   │   └── DeliveryRequestRepository
│   │
│   └── DependencyResolvers
│
├── API
│   ├── Controllers
│   │   ├── AuthController
│   │   ├── MovieController
│   │   ├── DeliveryRequestController
│   │   └── AccountingController
│   │
│   └── Program.cs
│
├── MVC
│   ├── Controllers
│   ├── Areas
│   │   └── DashBoard
│   │       ├── Controllers
│   │       ├── Models
│   │       └── Views
│   │
│   ├── Services
│   ├── Filters
│   └── Views
│
└── FilmDukkani.sln
```

## 👤 Roller ve Yetkilendirme

Sistem rol bazlı çalışır ve JWT + Policy Authorization kullanır.

### Roller

- Admin  
- Accounting  
- Warehouse  
- Purchasing  
- Member  

Her rol yalnızca kendi sorumluluk alanındaki endpoint ve ekranlara erişebilir.

---

## 📦 İş Modeli

- Sistem abonelik (subscription) modeli ile çalışır  
- Gelir yalnızca üyelik paketlerinden elde edilir  
- Film veya kategori bazlı doğrudan satış bulunmaz  

Minimum film kuralı bilinçli bir tasarım kararı olarak **5 adet** uygulanmıştır.

---

## 🎥 Temel Özellikler

### Üye Tarafı

- Film kataloğu ve gelişmiş filtreleme  
- Kişisel film listeleri  
- Teslimat talebi oluşturma  
- Teslimat iptal talebi (admin onaylı)  
- Liste kilidi sistemi  
- Abonelik yönetimi  

### Yönetim Paneli (Dashboard)

- Teslimat ve depo yönetimi  
- Bozuk film takibi  
- Satın alma talepleri  
- Raf (shelf) sistemi  
- Muhasebe ve operasyonel raporlar  

---

## 📊 Muhasebe ve Raporlama

Muhasebe modülü aşağıdaki raporları üretir:

- Kar / Zarar Özeti  
- Üye Bazlı Kârlılık Raporu  
- Film Bazlı Operasyonel Rapor  
- Kategori Bazlı Operasyonel Rapor  

Film ve kategori raporlarında gelir ve kâr değerlerinin **0** görünmesi tasarım gereğidir.  
Gelir yalnızca aboneliklerden elde edilmektedir.

---

## 🔐 Güvenlik

- JWT Authentication  
- Role & Policy bazlı erişim kontrolü  
- Hassas alanlar DTO üzerinden dışarı açılmaz  
- Entity seviyesinde veri güvenliği uygulanır  

---

## ⚙️ Kurulum

1. Repository’yi klonlayın  
2. `appsettings.json` dosyasında bağlantı ayarlarını yapılandırın  
3. Migration’ları çalıştırın  
4. API ve MVC projelerini birlikte başlatın  

```bash
Update-Database
```
## 👨‍💻 Geliştirici

Ali Demir
