using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
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
                    Status = false,
                    TotalPrice = 0,
                    PaymentMethod = "VnPay",
                    LastUpdateDate= DateTime.Now,
                    Notes="Thanh toan hoa don thanh cong"

                };
                context.Orders.Add(order);
                context.SaveChanges(); // ADD TRƯỚC ORDER ĐỂ TRÁNH ENTITY CONFLICT

                float totalPrice = 0;

                //   context.SaveChanges();

                foreach (var cartItem in cartItems)
                {
                    totalPrice += cartItem.Price * cartItem.quantity;
                    var orderDetail = new OrderDetail
                    {// Use the generated OrderID
                        OrderId = order.Id,
                        TicketDate = cartItem.TicketDate,
                        Quantity = cartItem.quantity,
                        UsedTicket= 0,
                        TicketId = cartItem.Id,
                        
                    };

                    context.OrderDetails.Add(orderDetail);
                }
                order.TotalPrice = totalPrice;
                context.Entry(order).State = EntityState.Modified; // TÍNH RA TỔNG SỐ TIỀN CUỐI CÙNG 
                context.SaveChanges();
                return order.Id;


            }
        }

        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            using (var context = new ZoomaContext())
            {
                return context.OrderDetails.Where(o => o.OrderId == orderId).ToList();
            }
        }

        public List<Order> GetOrdersByCustomerId(int userID)
        {
            using (var context = new ZoomaContext())
            {
                return context.Orders.Where(o => o.UserId == userID).ToList();
            }

        }

        public Order GetOrdersById(int orderId)
        {
            using (var context = new ZoomaContext())
            {
                return context.Orders.SingleOrDefault(o => o.Id == orderId);
            }

        }

        public void updateRefundOrder(int orderId)
        {
            using (var context = new ZoomaContext())
            {
                try
                {
                    var order = context.Orders.SingleOrDefault(o => o.Id == orderId);
                    if( order != null )
                    {
                        order.Notes = "Refund order";
                        context.Entry(order).State = EntityState.Modified;
                        context.SaveChanges();
                    }                   
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }
    }
}
