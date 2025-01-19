using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.WebApi.Models.Order
{
    public class UpdateOrderRequest
    {
        [Required(ErrorMessage = "Sipariş tarihi zorunludur.")]
        public DateTime OrderDate { get; set; }

        [Required(ErrorMessage = "Sipariş durumu zorunludur.")]
        [EnumDataType(typeof(OrderStatus), ErrorMessage = "Geçersiz sipariş durumu.")]
        public OrderStatus Status { get; set; }

        [Required(ErrorMessage = "Sipariş ürünleri zorunludur.")]
        [MinLength(1, ErrorMessage = "En az bir ürün seçilmelidir.")]
        public List<CartItem> CartItems { get; set; }
    }
}