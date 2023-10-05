using VNPayDemo;
using zooma_api.DTO;
using zooma_api.Models;

namespace Repositories
{
    public interface IOrderRepository
    {
        int CreateOrder(short customerId, List<CartItemDTO> cartItems);
        Order GetOrdersById(int orderId);
        List<Order> GetOrdersByCustomerId(int customerID);
        List<OrderDetail> GetOrderDetailsByOrderId(int orderId);

        void updateRefundOrder(int orderId);

    }
}
