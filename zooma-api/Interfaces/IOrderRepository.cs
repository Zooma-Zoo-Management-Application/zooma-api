using VNPayDemo;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IOrderRepository
    {
        int CreateOrder(short customerId, List<CartItemDTO> cartItems);
        Order GetOrdersById(int orderId);
        List<Order> GetOrdersByCustomerId(short customerID);
        List<OrderDetail> GetOrderDetailsByOrderId(int orderId);

        void updateRefundOrder(int orderId);

        IEnumerable<Order> GetFiveRecentOrders(); // ASYNCHRONOUS METHOD ???


        int GetQuantityOfSuccessOrders();

        Task<List<Order>> GetAllSuccessOrders();
    }
}
