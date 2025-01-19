using OnlineShoppingPlatformApp.Business.Operations.Product.Dtos;
using OnlineShoppingPlatformApp.Business.Types;
using OnlineShoppingPlatformApp.Data.Entities;
using OnlineShoppingPlatformApp.Data.Repositories;
using OnlineShoppingPlatformApp.Data.UnitOfWork;
using Microsoft.EntityFrameworkCore;

namespace OnlineShoppingPlatformApp.Business.Operations.Product
{
    public class ProductManager : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<ProductEntity> _productRepository;

        public ProductManager(IUnitOfWork unitOfWork, IRepository<ProductEntity> productRepository)
        {
            _unitOfWork = unitOfWork;
            _productRepository = productRepository;
        }

        public async Task<ServiceMessage<ProductEntity>> AddProduct(AddProductDto addProductDto)
        {
            var hasProduct = await _productRepository.GetAll(x => x.ProductName.ToLower() == addProductDto.Name.ToLower()).AnyAsync();
            if (hasProduct)
            {
                return new ServiceMessage<ProductEntity>
                {
                    IsSuccess = false,
                    Message = "Ürün zaten mevcut."
                };
            }

            var product = new ProductEntity
            {
                ProductName = addProductDto.Name,
                Price = addProductDto.Price,
                StockQuantity = addProductDto.Stock,
                Description = addProductDto.Description
            };

            _productRepository.Add(product);

            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceMessage<ProductEntity>
                {
                    IsSuccess = false,
                    Message = "Ürün eklenirken bir hata oluştu."
                };
            }

            return new ServiceMessage<ProductEntity>
            {
                IsSuccess = true,
                Message = "Ürün başarıyla eklendi.",
                Data = product
            };
        }

        public async Task<ServiceMessage> DeleteProduct(int id)
        {
            var product = await _productRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Ürün bulunamadı."
                };
            }

            _productRepository.Delete(product);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Ürün silinirken bir hata oluştu."
                };
            }

            return new ServiceMessage
            {
                IsSuccess = true,
                Message = "Ürün başarıyla silindi."
            };
        }

        public async Task<ServiceMessage<List<ProductEntity>>> GetAllProducts()
        {
            var products = await _productRepository.GetAll().ToListAsync();
            return new ServiceMessage<List<ProductEntity>>
            {
                IsSuccess = true,
                Message = "Ürünler başarıyla getirildi.",
                Data = products
            };
        }

        public async Task<ServiceMessage<ProductEntity>> GetProductById(int id)
        {
            var product = await _productRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return new ServiceMessage<ProductEntity>
                {
                    IsSuccess = false,
                    Message = "Ürün bulunamadı."
                };
            }
            return new ServiceMessage<ProductEntity>
            {
                IsSuccess = true,
                Message = "Ürün başarıyla getirildi.",
                Data = product
            };
        }

        public async Task<ServiceMessage<ProductEntity>> UpdateProduct(int id, UpdateProductDto updateProductDto)
        {
            var product = await _productRepository.GetAll(x => x.Id == id).FirstOrDefaultAsync();
            if (product == null)
            {
                return new ServiceMessage<ProductEntity>
                {
                    IsSuccess = false,
                    Message = "Ürün bulunamadı."
                };
            }
            product.ProductName = updateProductDto.Name;
            product.Price = updateProductDto.Price;
            product.StockQuantity = updateProductDto.Stock;
            product.Description = updateProductDto.Description;
            _productRepository.Update(product);
            try
            {
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new ServiceMessage<ProductEntity>
                {
                    IsSuccess = false,
                    Message = "Ürün güncellenirken bir hata oluştu."
                };
            }

            return new ServiceMessage<ProductEntity>
            {
                IsSuccess = true,
                Message = "Ürün başarıyla güncellendi.",
                Data = product
            };

        }
    }
}