using AuthServer.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data
{
    // AppDbContext : DbContext yapmamamızın sebebi Identity ile ilgili olan user tablolarınında
    // Product Category ile yani projemizle aynı db de olmasını istediğimizden.
    public class AppDbContext : IdentityDbContext<UserApp,IdentityRole,string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
                
        }

        //public DbSet<UserApp> Users yapmadık çünkü Identity Framework'ünden geliyor zaten bunlar.
        //Bir çok tablo geliyor Users,Roles,UserRoles gibi..
        public DbSet<Product> Products { get; set; }
        public DbSet<Basket> Basket { get; set; }
        public DbSet<BasketDetail> BasketDetail { get; set; }
        public DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            builder.Entity<Product>().Property(x => x.Name).IsRequired().HasMaxLength(200);
            builder.Entity<Product>().Property(x => x.Price).HasColumnType("decimal(18,2)");
            builder.Entity<Product>().HasIndex(x => x.Code).IsUnique();

            builder.Entity<UserRefreshToken>().HasKey(x => x.UserId);
            builder.Entity<UserRefreshToken>().Property(x => x.Code).IsRequired().HasMaxLength(200);

            builder.Entity<BasketDetail>().HasKey(x => new
            {
                x.BasketId,
                x.ProductId
            });


            base.OnModelCreating(builder);
        }

    }
}
