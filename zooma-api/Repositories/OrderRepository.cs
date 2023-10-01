using Repositories;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public int CreateOrder(short userID, List<CartItemDTO> cartItems)
        {
            using (var context = new ZoomaContext())
            {
                var order = new Order
                {
                    UserId = userID,
                    OrderDate = DateTime.Now,
                    Status = true,
                    TotalPrice = 0,
                    PaymentMethod = "VnPay"

                };
                float totalPrice = 0;
                context.Orders.Add(order);
                context.SaveChanges();
                foreach (var cartItem in cartItems)
                {
                    totalPrice += cartItem.Price * cartItem.quantity;
                    var orderDetail = new OrderDetail
                    {
                        OrderId = order.Id,
                        TicketDate = cartItem.TicketDate,
                        Quantity = cartItem.quantity,
                        UsedTicket= 0,// Use the generated OrderID
                        TicketId = cartItem.Id,
                        
                    };

                    context.OrderDetails.Add(orderDetail);
                }
                order.TotalPrice = totalPrice;
                context.Orders.Add(order);

                context.SaveChanges();
                return order.Id;


            }
        }

        public List<Order> GetOrdersByCustomerId(int customerID)
        {
            throw new NotImplementedException();
        }

        public List<Order> GetOrdersById(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
