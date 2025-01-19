using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OnlineShoppingPlatformApp.WebApi.Models.Order
{
    // Sepetteki her bir ürün ve miktarını temsil eden sınıf.
    // Örnek: Sepette 2 adet kalem (ProductId: 1, Quantity: 2) gibi.
    public class CartItem
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}