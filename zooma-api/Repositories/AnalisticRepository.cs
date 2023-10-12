using Microsoft.EntityFrameworkCore;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class AnalisticRepository : IAnalisticRepository
    {
        public float getTotalRevenues()
        {
            float value = 0;
            using (var context = new zoomadbContext())
            {
                try
                {
                    var orderDetails = context.OrderDetails.Include(o => o.Ticket).ToList();

                    foreach (var ticket in orderDetails)
                    {
                        value += ticket.Quantity * ticket.Ticket.Price;
                    }
                    return value;
                }
                catch (Exception)
                {

                    throw;
                }
                 
            }
        }



        public float RevenuesInDay
        {
            get
            {
                using (var _context = new zoomadbContext())
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        var monthlyOrderDetails = _context.OrderDetails
                                   .Where(od => od.TicketDate.Day == today.Day && od.TicketDate.Month == today.Month && od.TicketDate.Year == today.Year).Include(o => o.Ticket).ToList();

                        float revenue = 0;
                        foreach (var orderDetail in monthlyOrderDetails)
                        {
                            if (orderDetail.Order.Status)
                            {
                                revenue += orderDetail.Quantity * orderDetail.Ticket.Price;

                            }
                        }

                        return revenue;
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

            }
        }

        public TicketsQuantity TicketsQuantityInDay
        {
            get
            {
                using (var _context = new zoomadbContext())
                {
                    try
                    {
                        DateTime today = DateTime.Now;
                        var monthlyOrderDetails = _context.OrderDetails
                                   .Where(od => od.TicketDate.Day == today.Day && od.TicketDate.Month == today.Month && od.TicketDate.Year == today.Year).Include(o => o.Ticket).ToList();

                        int adult_ticket = 0;
                        int child_ticket = 0;
                        int senior_ticket = 0;
                        int total = 0;
                        foreach (var orderDetail in monthlyOrderDetails)
                        {
                            if (orderDetail.Order.Status)
                            {
                                total += orderDetail.Quantity;

                                if (orderDetail.TicketId == 1)
                                {
                                    adult_ticket += orderDetail.Quantity;
                                }
                                else if (orderDetail.TicketId == 2)
                                {
                                    child_ticket += orderDetail.Quantity;
                                }
                                else if (orderDetail.TicketId == 3)
                                {
                                    senior_ticket += orderDetail.Quantity;
                                }
                            }
                            
                        }

                        return new TicketsQuantity
                        {
                            Month = today.Month,
                            AdultTickets = adult_ticket,
                            ChildTickets = child_ticket,
                            SeniorTickets = senior_ticket,
                            TotalTickets = total,
                        };
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
        }

        public List<MonthRevenue> GetGetSixMonthsRevenues()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    List<MonthRevenue> monthlyRevenues = new List<MonthRevenue>();

                    DateTime today = DateTime.Now;

                    for (int i = 0; i < 6; i++)
                    {
                        DateTime currentMonth = today.AddMonths(-i); // LẤY THÁNG HIỆN TẠI RA VÀ GIẢM DẦN THEO FOR 
                        DateTime startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);// SET NGÀY ĐẦU THÁNG
                        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);// SET NGÀY CUỐI THÁNG

                        var monthlyOrderDetails = _context.OrderDetails
                            .Where(od => od.TicketDate >= startOfMonth && od.TicketDate <= endOfMonth).Include(o => o.Ticket).Include(o=>o.Order).ToList();

                        float monthlyRevenueValue = 0;
                        int quantity = 0;

                        foreach (var orderDetail in monthlyOrderDetails)
                        {
                            if (orderDetail.Order.Status)
                            {
                                monthlyRevenueValue += orderDetail.Ticket.Price * orderDetail.Quantity;
                                quantity += orderDetail.Quantity;
                            }

                                
                        }

                        monthlyRevenues.Add(new MonthRevenue
                        {
                            Month = startOfMonth.Month,
                            Revenue = monthlyRevenueValue,
                            TicketQuantity = quantity

                        });
                    }
                    return monthlyRevenues;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<TicketsQuantity> GetGetSixMonthsTicketQuantity()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    List<TicketsQuantity> monthlyTickets = new List<TicketsQuantity>();
                    DateTime today = DateTime.Now;

                    for (int i = 0; i < 6; i++)
                    {
                        DateTime currentMonth = today.AddMonths(-i); // LẤY THÁNG HIỆN TẠI RA VÀ GIẢM DẦN THEO FOR 
                        DateTime startOfMonth = new DateTime(currentMonth.Year, currentMonth.Month, 1);// SET NGÀY ĐẦU THÁNG
                        DateTime endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);// SET NGÀY CUỐI THÁNG

                        var monthlyOrderDetails = _context.OrderDetails
                            .Where(od => od.TicketDate >= startOfMonth && od.TicketDate <= endOfMonth).Include(o => o.Ticket).ToList();

                        int adult_ticket = 0;
                        int child_ticket = 0;
                        int senior_ticket = 0;
                        int total = 0;

                        foreach (var orderDetail in monthlyOrderDetails)
                        {
                            if (orderDetail.Order.Status)
                            {
                                total += orderDetail.Quantity;

                                if (orderDetail.TicketId == 1)
                                {
                                    adult_ticket += orderDetail.Quantity;
                                }
                                else if (orderDetail.TicketId == 2)
                                {
                                    child_ticket += orderDetail.Quantity;
                                }
                                else if (orderDetail.TicketId == 3)
                                {
                                    senior_ticket += orderDetail.Quantity;
                                }
                            }

                           
                        }

                        monthlyTickets.Add(new TicketsQuantity
                        {
                            Month = startOfMonth.Month,
                            AdultTickets = adult_ticket,
                            ChildTickets = child_ticket,
                            SeniorTickets = senior_ticket,
                            TotalTickets = total,

                        }); ;

                    }

                    return monthlyTickets;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }

    public class MonthRevenue
    {
        public int Month { get; set; }
        public float Revenue { get; set; }

        public int TicketQuantity { get; set; }
    }

    public class TicketsQuantity
    {
        public int Month { get; set; }
        public int AdultTickets { get; set; }
        public int ChildTickets { get; set; }
        public int SeniorTickets { get; set; }
        public int TotalTickets { get; set; }

    }
}
