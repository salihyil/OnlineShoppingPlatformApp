using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OnlineShoppingPlatformApp.Data.Enums;

namespace OnlineShoppingPlatformApp.Business.Operations.Order.Dtos
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
    }
}