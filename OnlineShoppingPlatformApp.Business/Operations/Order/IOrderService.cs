using OnlineShoppingPlatformApp.Business.Operations.Order.Dtos;
using OnlineShoppingPlatformApp.Business.Types;

namespace OnlineShoppingPlatformApp.Business.Operations.Order
{
    public interface IOrderService
    {
        Task<ServiceMessage<IEnumerable<OrderInfoDto>>> GetUserOrders(int userId);
        Task<ServiceMessage<IEnumerable<OrderInfoDto>>> GetAllOrders();
        Task<OrderInfoDto?> GetOrderById(int id);
        Task<ServiceMessage> CreateOrder(CreateOrderDto order);
        Task<ServiceMessage> UpdateOrder(int id, UpdateOrderDto order);
        Task<ServiceMessage> DeleteOrder(int id);
        Task<ServiceMessage> UpdateOrderStatus(int id, UpdateOrderStatusDto orderStatus);
    }
}