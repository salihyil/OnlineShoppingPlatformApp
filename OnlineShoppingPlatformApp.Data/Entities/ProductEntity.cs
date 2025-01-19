using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShoppingPlatformApp.Data.Entities
{
    public class ProductEntity : BaseEntity
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public string Description { get; set; }

        // junction(bağlantı) tablosudur ve Product ile Order arasındaki many-to-many (çok-çoka) ilişkiyi temsil eder. 
        public ICollection<OrderProductEntity> OrderProducts { get; set; }
    }

    public class ProductConfiguration : BaseConfiguration<ProductEntity>
    {
        public override void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.ProductName).IsRequired().HasMaxLength(100);
            builder.Property(x => x.Price).IsRequired().HasPrecision(18, 2);
            builder.Property(x => x.StockQuantity).IsRequired();
        }
    }
}