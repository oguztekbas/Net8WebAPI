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
using AuthServer.Service.HelperMethods;
using AuthServer.Service.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

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


builder.Services.AddSingleton<RedisService>(sp =>
{
    return new RedisService(builder.Configuration.GetSection("CacheOptions:Url").Value);
});


////////////////////////////////////////////////////////////////////////////////////////




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

//app.UseHttpsRedirection();

//------------------------------------------------------------ Authentication middleware eklendi.
app.UseAuthentication();
//------------------------------------------------------------

app.UseAuthorization();

//--------------------------------------------------------- GlobalException middleware Extension'� eklendi.
app.UseHandleGlobalException();
//---------------------------------------------------------


app.MapControllers();

app.Run();
