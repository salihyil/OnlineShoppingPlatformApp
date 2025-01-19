using System.Collections.Generic;

namespace OnlineShoppingPlatformApp.Business.Operations.Order.Dtos
{
    public class CreateOrderDto
    {
        // OrderDate server tarafında set edilecek
        // Status her zaman Pending olarak başlayacak
        // TotalAmount server tarafında hesaplanacak
        public List<CartItemDto> CartItems { get; set; }
        public int UserId { get; set; } // Controller'da set edilecek, client'tan gelmeyecek
    }
}