using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.WebApi.Models.Order
{
    public class CreateOrderRequest
    {
        [Required(ErrorMessage = "Sipariş ürünleri zorunludur.")]
        [MinLength(1, ErrorMessage = "En az bir ürün seçilmelidir.")]
        public List<CartItem> CartItems { get; set; }
    }
}