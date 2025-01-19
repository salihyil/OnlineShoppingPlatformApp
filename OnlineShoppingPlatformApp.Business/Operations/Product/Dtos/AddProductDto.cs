namespace OnlineShoppingPlatformApp.Business.Operations.Product.Dtos
{
    public class AddProductDto
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    }
}