using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.Data.Entities
{
    public class OrderEntity : BaseEntity
    {
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus Status { get; set; }

        public int UserId { get; set; }

        // Navigation Property (One-to-Many için):
        public UserEntity? User { get; set; }

        // Navigation Property (Many-to-Many için):
        public ICollection<OrderProductEntity> OrderProducts { get; set; } 
    }

    public class OrderConfiguration : BaseConfiguration<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.OrderDate).IsRequired();
            builder.Property(x => x.TotalAmount).IsRequired().HasPrecision(18, 2);
        }
    }
}