# .NET8WebAPI

 Kullanılan teknoloji, teknik ve kütüphaneler:

# => .Net 8 Web API - JWT - Repository Pattern ile Katmanlı Mimari - Depency Injection - Global Exception Handler - Custom Middlewares
**********************************************************
# => UnitOfWork - Entity Framework Core - Migrations - Identity Framework - Options Pattern - AutoMapper - LINQ Queries
*********************************************************
# => Redis Caching - Request/Response Logging and Global Exception Errors Logging With SeriLog to ElasticSearch and File

# => RabbitMQ - BackgroundService - EmailSend

**********************************************************
- Katmanlı Mimari ve Repository Pattern yapısında, JWT token dağıtan, Entity Framework Core ve Redis Distributed Cache kullanan, RabbitMQ ve BackgroundService ile asenkron mail gönderen, SeriLog ile ElasticSearch'e log atma mekanizmasına sahip olan Basic bir .NET 8 WEB API şablonu.
- AccessToken RefreshToken RevokeRefreshToken işlemleri

**********************************************************
**********************************************************
- API => Ana projemiz - Controller kısımları sade yazıldı. Bu yüzden kod yükü diğer katmanlarda. Program.cs'te Depency Injection'lar ve configler var. Exception ve RequestResponse'u yakaladığımız ve Serilog ile Elasticsearch'e log attırdığımız Custom Middlewares yapıları var.
  Kullanıcı login olduğunda business katmanında RabbitMQ ile kuyruğa directRouting yöntemiyle emailText mesajını iletiyoruz API tarafında ise BackgroundService'miz var uygulamada her an çalışan burada consume edip mail gönderiyoruz.   ---Service katmanını referans alır.
  
 
- Service => Business Logic ve Validation işlemleri ve Redis Caching'i kullanır.  ---  Data katmanını referans alır
- Data => DB'ye erişim, sorgular ve migrationlar
- Core => Interface ve class'lar
- Cache => Redis implementasyonu
- RabbitMQ => RabbitMQ implementasyonu
- EmailService => Email implementasyonu



