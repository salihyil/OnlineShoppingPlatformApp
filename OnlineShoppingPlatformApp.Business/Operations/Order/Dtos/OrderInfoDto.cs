using System;
using System.Collections.Generic;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.Business.Operations.Order.Dtos
{
    public class OrderInfoDto
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int UserId { get; set; }
        public OrderStatus Status { get; set; }
        public List<OrderProductDto> Products { get; set; }

    }

   
}