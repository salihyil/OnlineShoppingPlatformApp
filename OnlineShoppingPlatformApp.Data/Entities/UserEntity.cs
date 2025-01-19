using OnlineShoppingPlatformApp.Data.Enums;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShoppingPlatformApp.Data.Entities
{
    public class UserEntity : BaseEntity
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public UserType Role { get; set; }

        //  bir navigation property'dir ve User ile Order arasındaki one-to-many (bir-çok) ilişkiyi temsil eder.
        public ICollection<OrderEntity>? Orders { get; set; } // Kullanıcının siparişleri
    }

    public class UserConfiguration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            base.Configure(builder);

            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.HasIndex(u => u.Email).IsUnique();
        }
    }
}