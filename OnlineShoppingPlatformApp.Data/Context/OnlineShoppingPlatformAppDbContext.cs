using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatformApp.Data.Entities;

namespace OnlineShoppingPlatformApp.Data.Context
{
    public class OnlineShoppingPlatformAppDbContext : DbContext
    {
        public OnlineShoppingPlatformAppDbContext(DbContextOptions<OnlineShoppingPlatformAppDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Fluent API kullanarak veritabanı tablolarının oluşturulması ve ilişkilerinin belirlenmesi
            // configurasyonları burdan yapmak istemiyorum.
            // gibi -> modelBuilder.Entity<UserEntity>().Property(x => x.FirstName).IsRequired().HasMaxLength(100);
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderProductConfiguration());

            //seed data
            modelBuilder.Entity<SettingEntity>().HasData(new SettingEntity { Id = 1, MaintenanceMode = false });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<ProductEntity> Products => Set<ProductEntity>();
        public DbSet<OrderEntity> Orders => Set<OrderEntity>();
        public DbSet<OrderProductEntity> OrderProducts => Set<OrderProductEntity>();
        public DbSet<SettingEntity> Settings => Set<SettingEntity>();
    }
}