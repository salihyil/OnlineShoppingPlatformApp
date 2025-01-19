using System;
using System.Collections.Generic;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.Business.Operations.Order.Dtos
{
    public class UpdateOrderDto
    {
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public List<CartItemDto> CartItems { get; set; }
    }
}