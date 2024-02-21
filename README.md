# .NET8WebAPI

 Kullanılan teknoloji, teknik ve kütüphaneler:

.Net 8 Web API - JWT - Repository Pattern ile Katmanlı Mimari - Depency Injection - Global Exception Handler - UnitOfWork - EF Core 8 - Migrations - Options Pattern - AutoMapper - Identity Framework

- Katmanlı Mimari yapısında, Generic Repository Pattern ve .Net 8 kullanan, JWT token dağıtan, Basic bir WEB API şablonu.
- Hem email ve password ile login amaçlı request gönderen ve token isteyen bir client için hem de üyelik sistemi gerektirmeyen ancak güvenlik için token isteyen, clientId ve clientSecret bilgileriyle gelen bir client için (Örn: Hava durumu Client app'i) token dağıtan API 






- API => Ana projemiz - Controller kısımları sade yazıldı. Kod yükü diğer katmanlarda sadece program.cs'te Depency Injection'lar ve configler var  ---  Service katmanını referans alır.
- Service => Business Logic ve Validation işlemleri ---  Data katmanını referans alır
- Data => DB'ye erişim, sorgular ve migrationlar
- Core => Interface ve class'lar


- AccessToken RefreshToken RevokeRefreshToken
