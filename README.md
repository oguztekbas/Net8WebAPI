# .NET8WebAPI

 Kullanılan teknoloji, teknik ve kütüphaneler:

# => .Net 8 Web API - JWT - Repository Pattern ile Katmanlı Mimari - Depency Injection - Global Exception Handler - Custom Middlewares
**********************************************************
# => UnitOfWork - Entity Framework Core - Migrations - Identity Framework - Options Pattern - AutoMapper
*********************************************************
# => Redis Caching - Request-Response and Global Exception Logging With SeriLog to ElasticSearch and File

**********************************************************
- Katmanlı Mimari yapısında, Generic Repository Pattern, JWT token dağıtan, Redis Caching kullanan, SeriLog ile ElasticSearch'e log atma mekanizmasına sahip olan Basic bir .NET 8 WEB API şablonu.
- AccessToken RefreshToken RevokeRefreshToken işlemleri 

**********************************************************
**********************************************************
- API => Ana projemiz - Controller kısımları sade yazıldı.Bu yüzden kod yükü diğer katmanlarda. Sadece program.cs'te Depency Injection'lar ve configler var bunun dışında Custom Middlewares yapıları var ---  Service katmanını referans alır.
- Service => Business Logic ve Validation işlemleri ve Redis Caching'i kullanır.  ---  Data katmanını referans alır
- Data => DB'ye erişim, sorgular ve migrationlar
- Core => Interface ve class'lar
- Cache => Redis implementasyonu


# Eklenecekler
*****************************************
- RabbitMQ ile email gönderme veya pdf creator işlemi
