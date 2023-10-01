using VNPayDemo;
using zooma_api.DTO;
using zooma_api.Models;

namespace Repositories
{
    public interface IOrderRepository
    {
        int CreateOrder(short customerId, List<CartItemDTO> cartItems);
        List<Order> GetOrdersById(int orderId);
        List<Order> GetOrdersByCustomerId(int customerID);
    }
}
