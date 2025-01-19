using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShoppingPlatformApp.Data.Entities
{
    public class BaseEntity
    {

        public int Id { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }

    // abstract class görevi yalnızca miras vermek olacak. abstrack nesne oluşturulamaz.
    public abstract class BaseConfiguration<TEntiy> : IEntityTypeConfiguration<TEntiy> where TEntiy : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntiy> builder)
        {
            //soft delete işleminde bir sürü linq sorgularımız olacak. her seferinde isDeleted == false işlemleri yapmak yerine bu işlemi burada yapacağız.
            //bütün linqlerde bu işlemi yapmamızı sağlayacak.
            //böylece soft delete işlemlerimizi daha kolay yapabileceğiz.
            builder.HasQueryFilter(x => x.IsDeleted == false);

            // bütün tablolarda olacak olan özellikler
            builder.Property(x => x.ModifiedDate).IsRequired(false);
        }
    }
}
