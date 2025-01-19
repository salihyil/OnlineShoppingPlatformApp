using System.ComponentModel.DataAnnotations;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.WebApi.Models.Order
{
    public class UpdateOrderStatusRequest
    {
        [Required]
        [EnumDataType(typeof(OrderStatus))]
        public OrderStatus Status { get; set; }
    }
}