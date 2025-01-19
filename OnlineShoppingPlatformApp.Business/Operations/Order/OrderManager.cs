using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatformApp.Business.Operations.Order.Dtos;
using OnlineShoppingPlatformApp.Business.Types;
using OnlineShoppingPlatformApp.Data.Entities;
using OnlineShoppingPlatformApp.Data.Enums;
using OnlineShoppingPlatformApp.Data.Repositories;
using OnlineShoppingPlatformApp.Data.UnitOfWork;

namespace OnlineShoppingPlatformApp.Business.Operations.Order
{
    public class OrderManager : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRepository<OrderEntity> _orderRepository;
        private readonly IRepository<OrderProductEntity> _orderProductRepository;
        private readonly IRepository<ProductEntity> _productRepository;


        public OrderManager(IUnitOfWork unitOfWork, IRepository<OrderEntity> orderRepository, IRepository<OrderProductEntity> orderProductRepository, IRepository<ProductEntity> productRepository)
        {
            _unitOfWork = unitOfWork;
            _orderRepository = orderRepository;
            _orderProductRepository = orderProductRepository;
            _productRepository = productRepository;

        }

        public async Task<ServiceMessage> CreateOrder(CreateOrderDto order)
        {
            try
            {
                // 1. Temel validasyonlar
                if (order.CartItems == null || order.CartItems.Count == 0)
                    return new ServiceMessage
                    {
                        IsSuccess = false,
                        Message = "Sipariş için en az bir ürün seçilmelidir."
                    };

                // 2. Ürünleri tek seferde yükle (N+1 probleminden kaçınmak için)
                var productIds = order.CartItems.Select(x => x.ProductId).Distinct().ToList();
                var products = await _productRepository.GetAll(p => productIds.Contains(p.Id))
                    .ToDictionaryAsync(p => p.Id, p => p);

                // 3. Ürün validasyonları
                if (products.Count != productIds.Count)
                {
                    var missingProducts = productIds.Where(id => !products.ContainsKey(id));
                    return new ServiceMessage
                    {
                        IsSuccess = false,
                        Message = $"Bazı ürünler bulunamadı. Bulunamayan ürün ID'leri: {string.Join(", ", missingProducts)}"
                    };
                }

                // 4. Stok kontrolü ve toplam tutar hesaplama
                decimal calculatedTotal = 0;
                var stockErrors = new List<string>();

                foreach (var cartItem in order.CartItems)
                {
                    var product = products[cartItem.ProductId];
                    if (product.StockQuantity < cartItem.Quantity)
                    {
                        stockErrors.Add($"{product.ProductName} için yeterli stok yok. İstenen: {cartItem.Quantity}, Mevcut: {product.StockQuantity}");
                        continue;
                    }
                    calculatedTotal += product.Price * cartItem.Quantity;
                }

                if (stockErrors.Any())
                    return new ServiceMessage
                    {
                        IsSuccess = false,
                        Message = $"Stok yetersizliği:\n{string.Join("\n", stockErrors)}"
                    };

                // 5. Transaction başlat
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    // 6. Sipariş oluştur
                    var orderEntity = new OrderEntity
                    {
                        OrderDate = DateTime.UtcNow, // Server tarafında set ediyoruz
                        TotalAmount = calculatedTotal, // Server tarafında hesaplıyoruz
                        Status = OrderStatus.Pending, // Her zaman Pending başlıyor
                        UserId = order.UserId
                    };

                    _orderRepository.Add(orderEntity);
                    await _unitOfWork.SaveChangesAsync();

                    // 7. Sipariş detaylarını oluştur ve stokları güncelle
                    foreach (var cartItem in order.CartItems)
                    {
                        var product = products[cartItem.ProductId];

                        var orderProduct = new OrderProductEntity
                        {
                            OrderId = orderEntity.Id,
                            ProductId = cartItem.ProductId,
                            Quantity = cartItem.Quantity,
                            UnitPrice = product.Price // Sipariş anındaki fiyatı saklıyoruz
                        };
                        _orderProductRepository.Add(orderProduct);

                        // Stok güncelleme
                        product.StockQuantity -= cartItem.Quantity;
                        _productRepository.Update(product);
                    }

                    await _unitOfWork.SaveChangesAsync();
                    await _unitOfWork.CommitTransactionAsync();

                    return new ServiceMessage
                    {
                        IsSuccess = true,
                        Message = "Sipariş başarıyla oluşturuldu.",
                        Data = new { OrderId = orderEntity.Id }
                    };
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new Exception("Sipariş kaydedilirken bir hata oluştu.", ex);
                }
            }
            catch (Exception ex)
            {
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = $"Sipariş oluşturulurken bir hata oluştu: {ex.Message}"
                };
            }
        }

        public async Task<ServiceMessage> DeleteOrder(int id)
        {
            var order = await _orderRepository.GetAll(o => o.Id == id)
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync();

            if (order == null)
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Sipariş bulunamadı."
                };

            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Return products to stock
                foreach (var orderProduct in order.OrderProducts)
                {
                    var product = _productRepository.GetById(orderProduct.ProductId);
                    if (product != null)
                    {
                        product.StockQuantity += orderProduct.Quantity;
                        _productRepository.Update(product);
                    }
                }

                _orderRepository.Delete(order);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ServiceMessage
                {
                    IsSuccess = true,
                    Message = "Sipariş başarıyla silindi."
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = "Sipariş silinirken bir hata oluştu: " + ex.Message
                };
            }
        }

        public async Task<ServiceMessage<IEnumerable<OrderInfoDto>>> GetAllOrders()
        {
            var orders = await _orderRepository.GetAll().Select(o => new OrderInfoDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                UserId = o.UserId,
                Status = o.Status,
                Products = o.OrderProducts.Select(op => new OrderProductDto
                {
                    Id = op.ProductId,
                    Quantity = op.Quantity,
                    ProductName = op.Product.ProductName,
                    Price = op.Product.Price
                }).ToList()
            }).ToListAsync();

            return new ServiceMessage<IEnumerable<OrderInfoDto>>
            {
                IsSuccess = true,
                Message = "Siparişler başarıyla getirildi.",
                Data = orders
            };
        }

        public async Task<OrderInfoDto?> GetOrderById(int id)
        {
            var order = await _orderRepository.GetAll(o => o.Id == id).Select(o => new OrderInfoDto
            {
                Id = o.Id,
                OrderDate = o.OrderDate,
                TotalAmount = o.TotalAmount,
                UserId = o.UserId,
                Status = o.Status,
                Products = o.OrderProducts.Select(op => new OrderProductDto
                {
                    Id = op.ProductId,
                    Quantity = op.Quantity,
                    ProductName = op.Product.ProductName,
                    Price = op.Product.Price
                }).ToList()
            }).FirstOrDefaultAsync();

            return order;
        }

        public async Task<ServiceMessage<IEnumerable<OrderInfoDto>>> GetUserOrders(int userId)
        {
            var orders = await _orderRepository.GetAll(order => order.UserId == userId).Select(order => new OrderInfoDto
            {
                Id = order.Id,
                OrderDate = order.OrderDate,
                TotalAmount = order.TotalAmount,
                UserId = order.UserId,
                Status = order.Status,
                Products = order.OrderProducts.Select(op => new OrderProductDto
                {
                    Id = op.ProductId,
                    Quantity = op.Quantity,
                    ProductName = op.Product.ProductName,
                    Price = op.Product.Price
                }).ToList()
            }).ToListAsync();

            return new ServiceMessage<IEnumerable<OrderInfoDto>>
            {
                IsSuccess = true,
                Message = "Siparişler başarıyla getirildi.",
                Data = orders
            };
        }

        public async Task<ServiceMessage> UpdateOrder(int id, UpdateOrderDto order)
        {
            // 1. Sipariş kontrolü
            var orderEntity = await _orderRepository.GetAll(o => o.Id == id)
                .Include(o => o.OrderProducts)
                .FirstOrDefaultAsync();

            if (orderEntity == null)
                return new ServiceMessage { IsSuccess = false, Message = "Sipariş bulunamadı." };

            // 2. Ürünleri tek seferde yükle
            var productIds = order.CartItems.Select(x => x.ProductId).Distinct().ToList();
            var products = await _productRepository.GetAll(p => productIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p);

            // 3. Ürün validasyonları
            if (products.Count != productIds.Count)
            {
                var missingProducts = productIds.Where(id => !products.ContainsKey(id));
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = $"Bazı ürünler bulunamadı. Bulunamayan ürün ID'leri: {string.Join(", ", missingProducts)}"
                };
            }

            // 4. Stok kontrolü ve toplam tutar hesaplama
            decimal calculatedTotal = 0;
            var stockErrors = new List<string>();

            // Önce eski stokları geri ekle
            foreach (var oldOrderProduct in orderEntity.OrderProducts)
            {
                var product = products.GetValueOrDefault(oldOrderProduct.ProductId);
                if (product != null)
                {
                    product.StockQuantity += oldOrderProduct.Quantity;
                }
            }

            // Yeni stok kontrolü
            foreach (var cartItem in order.CartItems)
            {
                var product = products[cartItem.ProductId];
                if (product.StockQuantity < cartItem.Quantity)
                {
                    stockErrors.Add($"{product.ProductName} için yeterli stok yok. İstenen: {cartItem.Quantity}, Mevcut: {product.StockQuantity}");
                    continue;
                }
                calculatedTotal += product.Price * cartItem.Quantity;
            }

            if (stockErrors.Any())
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = $"Stok yetersizliği:\n{string.Join("\n", stockErrors)}"
                };

            // 5. Transaction başlat
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                // 6. Siparişi güncelle
                orderEntity.OrderDate = order.OrderDate;
                orderEntity.TotalAmount = calculatedTotal; // Server tarafında hesaplıyoruz
                orderEntity.Status = order.Status;

                // 7. OrderProduct kayıtlarını güncelle
                foreach (var orderProduct in orderEntity.OrderProducts)
                {
                    _orderProductRepository.Delete(orderProduct);
                }

                foreach (var cartItem in order.CartItems)
                {
                    var product = products[cartItem.ProductId];

                    // Stok düş
                    product.StockQuantity -= cartItem.Quantity;
                    _productRepository.Update(product);

                    var orderProduct = new OrderProductEntity
                    {
                        OrderId = orderEntity.Id,
                        ProductId = cartItem.ProductId,
                        Quantity = cartItem.Quantity,
                        UnitPrice = product.Price // Güncel fiyatı kullan
                    };
                    _orderProductRepository.Add(orderProduct);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                return new ServiceMessage
                {
                    IsSuccess = true,
                    Message = "Sipariş başarıyla güncellendi.",
                    Data = new { OrderId = orderEntity.Id }
                };
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return new ServiceMessage
                {
                    IsSuccess = false,
                    Message = $"Güncelleme sırasında hata: {ex.Message}"
                };
            }
        }

        public async Task<ServiceMessage> UpdateOrderStatus(int id, UpdateOrderStatusDto orderStatus)
        {
            var order = _orderRepository.GetById(id);
            if (order == null)
                return new ServiceMessage { IsSuccess = false, Message = "Sipariş bulunamadı." };

            order.Status = orderStatus.Status;
            _orderRepository.Update(order);

            try
            {
                await _unitOfWork.SaveChangesAsync();
                return new ServiceMessage { IsSuccess = true, Message = "Sipariş durumu başarıyla güncellendi." };
            }
            catch (Exception ex)
            {
                return new ServiceMessage { IsSuccess = false, Message = $"Güncelleme sırasında hata: {ex.Message}" };
            }
        }
    }
}