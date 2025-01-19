using System.ComponentModel.DataAnnotations;

namespace OnlineShoppingPlatformApp.WebApi.Models.Product
{
    public class AddProductRequest
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(1000)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int Stock { get; set; }
    }
}