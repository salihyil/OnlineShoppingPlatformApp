using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingPlatformApp.Business.Operations.Order.Dtos
{
    public class OrderProductDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }

    }
}