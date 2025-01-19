using OnlineShoppingPlatformApp.Business.Operations.Product.Dtos;
using OnlineShoppingPlatformApp.Business.Types;
using OnlineShoppingPlatformApp.Data.Entities;

namespace OnlineShoppingPlatformApp.Business.Operations.Product
{
    public interface IProductService
    {
        Task<ServiceMessage<List<ProductEntity>>> GetAllProducts();
        Task<ServiceMessage<ProductEntity>> GetProductById(int id);
        Task<ServiceMessage<ProductEntity>> AddProduct(AddProductDto addProductDto);
        Task<ServiceMessage<ProductEntity>> UpdateProduct(int id, UpdateProductDto updateProductDto);
        Task<ServiceMessage> DeleteProduct(int id);
    }
}