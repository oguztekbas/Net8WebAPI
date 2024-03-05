using AuthServer.API.BackgroundServices;
using AuthServer.API.Extensions;
using AuthServer.Cache;
using AuthServer.Core.Entities;
using AuthServer.Core.Models;
using AuthServer.Core.Repositories;
using AuthServer.Core.Services;
using AuthServer.Core.UnitOfWork;
using AuthServer.Data;
using AuthServer.Data.Repositories;
using AuthServer.Data.UnitOfWork;
using AuthServer.RabbitMQ;
using AuthServer.Service.HelperMethods;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RabbitMQ.Client;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);



////////////////////////////////////////////////////////////////////////////////////////


//DI Register 
// AddScoped => Ayn� istekte birden fazla ayn� interface ile kar��la��rsa ayn� object �rne�ini kullan�r.
// AddTransient => Ayn� istekte birden fazla ayn� interface ile kar��la��rsa yeni bir object �rne�i kullan�r.,
// AddSignleton => Uygulama boyunca tek bir object �rne�ini kullan�r.

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>)); //Generic oldu�u i�in imp'i farkl�.
//builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>)); //Birden fazla tip alan Generic oldu�u i�in <,> yapt�k.

builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddScoped<IBasketRepository, BasketRepository>();
builder.Services.AddScoped<IBasketService, BasketService>();

builder.Services.AddScoped<IBasketDetailRepository, BasketDetailRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Add DBContext
builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer"),sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("AuthServer.Data");
    });
});

// Identity ayarlamalar�
builder.Services.AddIdentity<UserApp, IdentityRole>(Opt =>
{
    Opt.User.RequireUniqueEmail = true;
    Opt.Password.RequireNonAlphanumeric = false;
}).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();


// Authentication ayarlamalar� Token do�rulamala i�lemleri
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, opts =>
{
    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,
        ValidateAudience = true,
        ValidateIssuer = true,
        ValidateLifetime = true,

        ValidIssuer = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>().Issuer,
        ValidAudience = builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>().Audience[0],
        IssuerSigningKey = SecurityMethods.GetSymmetricSecurityKey(builder.Configuration.GetSection("TokenOption").Get<CustomTokenOption>().SecurityKey),

        ClockSkew = TimeSpan.Zero //
    };
});


//OptionsPattern => appSettings'deki datay� C# objesi olarak kullanabilmek i�in
builder.Services.Configure<CustomTokenOption>(builder.Configuration.GetSection("TokenOption"));
builder.Services.Configure<List<ClientTokenOption>>(builder.Configuration.GetSection("Clients"));


//RedisService constructor'u url parametresi bekledi�i i�in bu �ekilde koyduk.
//Parametreden string url yerine bi �stteki gibi OptionsPattern ile de alabilirdik
builder.Services.AddSingleton<RedisService>(sp =>
{
    return new RedisService(builder.Configuration.GetSection("CacheOptions:Url").Value);
});


// Serilog k�s�mlar�
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .Enrich.FromLogContext()
    .WriteTo.Async(c => c.Console())
    .CreateLogger();

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.Enrich.FromLogContext()
        .ReadFrom.Configuration(context.Configuration);
});

//SeriLog mekanizmas�n� Singleton olarak ekledin.
builder.Services.AddSingleton(Log.Logger);


//RabbitMQ
builder.Services.AddSingleton(sp =>
{
    return new ConnectionFactory()
    {
        Uri = new Uri(builder.Configuration.GetSection("ConnectionStrings:RabbitMQ").Value),
        DispatchConsumersAsync = true // bu �nemli ��nk� RabbitMQ consume etti�in yerdeki received metodunu async kulland���n i�in �al��m�yordu.
    };
});

builder.Services.AddSingleton<RabbitMQClientService>();
builder.Services.AddSingleton<RabbitMQPublisher>();

//RabbitMQ'yu consume edecek backgroundService
builder.Services.AddHostedService<EmailSendBackgroundService>();




////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); 

//-------------
app.UseHandleGlobalException();
app.UseMiddleware<RequestResponseMiddleware>();
//-------------

//-------------------------------Authentication middleware eklendi.
app.UseAuthentication();
//-------------------------------

app.UseAuthorization();


app.MapControllers();

app.Run();
