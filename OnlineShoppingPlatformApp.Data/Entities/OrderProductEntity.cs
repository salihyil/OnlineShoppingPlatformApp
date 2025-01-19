using System;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShoppingPlatformApp.Data.Entities
{
    public class OrderProductEntity : BaseEntity
    {
        // Siparişler ile ürünler arasında çoka çok bir ilişki kurun. Her sipariş birden fazla ürünü içerebilir ve her ürün birden fazla siparişte yer alabilir. Aşağıdaki özellikleri içermelidir:

        //Quantity (Miktar), bir siparişte kaç adet ürün alındığını temsil eder.
        public int Quantity { get; set; }

        // Sipariş anındaki ürün fiyatı
        public decimal UnitPrice { get; set; }

        // Navigation properties
        public OrderEntity? Order { get; set; }
        public int OrderId { get; set; }

        public ProductEntity? Product { get; set; }
        public int ProductId { get; set; }

    }

    public class OrderProductConfiguration : BaseConfiguration<OrderProductEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderProductEntity> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.OrderId).IsRequired();
            builder.Property(x => x.ProductId).IsRequired();
            builder.Property(x => x.UnitPrice).IsRequired().HasPrecision(18, 2);

            //1 siparişten birden çok ürün olabilir

            //üründe birden çok sipariş olabilir

            //çoktan çoka ilişkilerde bir ara tablo oluşturuyoduk.ve bu diğer 2 tablodan gelen 
            //foreign keyler composite(birleşik) key oluşturuyordu. yeni tablomuzun primary keyi oluyordu.

            // burda OrderId ve ProductId birleşik primary key oluşturuyor. BaseEntity'den gelen Id'yi ignore ediyoruz.
            builder.Ignore(x => x.Id); // id property'sini görmezden geldik. tabloya aktarılmayacak.
            builder.HasKey("OrderId", "ProductId");
            //composite key oluşturup yeni primary key olarak atadık.
        }
    }
}