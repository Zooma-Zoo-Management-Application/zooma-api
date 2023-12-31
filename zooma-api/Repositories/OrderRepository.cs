using AutoMapper.Execution;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        public int CreateOrder(short userID, List<CartItemDTO> cartItems)
        {
            using (var context = new zoomadbContext())
            {
                var order = new Order
                {
                    UserId = userID,
                    OrderDate = DateTime.UtcNow.AddHours(7),
                    Status = 1, // ĐANG THANH TOÁN 
                    TotalPrice = 0,
                    PaymentMethod = "VnPay",
                    LastUpdateDate= DateTime.UtcNow.AddHours(7),
                    Notes="Payment in progress"

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

        public int RepayOrder(short userID, Order oldOrder)
        {
            using (var context = new zoomadbContext())
            {
                var order = new Order
                {
                    UserId = userID,
                    OrderDate = DateTime.UtcNow.AddHours(7),
                    Status = 1, // ĐANG THANH TOÁN 
                    TotalPrice = oldOrder.TotalPrice,
                    PaymentMethod = "VnPay",
                    LastUpdateDate = DateTime.UtcNow.AddHours(7),
                    Notes = "Payment in progress",                 
                };
                context.Orders.Add(order);
                context.SaveChanges(); // ADD TRƯỚC ORDER ĐỂ TRÁNH ENTITY CONFLICT

                var oldOrderDetails = context.OrderDetails.Where(o=>o.OrderId == oldOrder.Id).ToList();

                //   context.SaveChanges();

                foreach (var details in oldOrderDetails)
                {
                    var orderDetail = new OrderDetail
                    {// Use the generated OrderID
                        OrderId = order.Id,
                        TicketDate = details.TicketDate,
                        Quantity = details.Quantity,
                        UsedTicket = 0,
                        TicketId = details.TicketId,
                    };

                    context.OrderDetails.Add(orderDetail);
                }
               // order.TotalPrice = totalPrice;
               //context.Entry(order).State = EntityState.Modified; // TÍNH RA TỔNG SỐ TIỀN CUỐI CÙNG 
                context.SaveChanges();
                return order.Id;


            }
        }

        public IEnumerable<Order> GetFiveRecentOrders()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var currentDate = DateTime.UtcNow.AddHours(7); // Lấy thời điểm hiện tại

                    var recentOrders = _context.Orders
                                        .Where(o => o.Status == 2 && o.LastUpdateDate <= currentDate) // Lọc các đơn hàng có Status=1 và OrderDate không lớn hơn thời điểm hiện tại
                                        .OrderByDescending(o => o.LastUpdateDate) // Sắp xếp theo OrderDate giảm dần để lấy các đơn hàng gần đây nhất trước
                                        .Take(5) // Lấy ra 5 đơn hàng
                                        .Include(o => o.OrderDetails)
                                        .Include(o => o.Transactions)
                                        .Include(o => o.User).ToList();
                                       

                    return recentOrders;

                    
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }

        public List<OrderDetail> GetOrderDetailsByOrderId(int orderId)
        {
            using (var context = new zoomadbContext())
            {
                return context.OrderDetails.Where(o => o.OrderId == orderId).Include(o => o.Ticket).ToList();
            }
        }

        public List<Order> GetOrdersByCustomerId(short userID)
        {
            using (var context = new zoomadbContext())
            {
                return context.Orders.Where(o => o.UserId == userID).Include(o=>o.OrderDetails).ThenInclude(o=>o.Ticket).Include(o=>o.Transactions).OrderByDescending(o => o.LastUpdateDate).ToList();
            }

        }

        public Order GetOrdersById(int orderId)
        {
            using (var context = new zoomadbContext())
            {
                try
                {
                    var order = context.Orders.Where(o => o.Id == orderId).Include(o => o.OrderDetails).ThenInclude(o=>o.Ticket).Include(o => o.Transactions).Include(o=>o.User).FirstOrDefault();
                    return order;
                }
                catch (Exception)
                {

                    throw;
                }
            }

        }

        public void updateRefundOrder(int orderId)
        {
            using (var context = new zoomadbContext())
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

        public async Task<List<Order>> GetAllSuccessOrders()
        {
            using (var context = new zoomadbContext())
            {
                try
                {
                    return await context.Orders.Where(o => o.Status == 2).Include(o => o.OrderDetails).ThenInclude(o => o.Ticket).Include(o => o.Transactions).OrderByDescending(o => o.LastUpdateDate).ToListAsync();
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        public int GetQuantityOfSuccessOrders()
        {
            using (var context = new zoomadbContext())
            {
                try
                {
                    var quantity = context.Orders.Where(o => o.Status == 2).ToList();
                    return quantity.Count;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }


    }
}
