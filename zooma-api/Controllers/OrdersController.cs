using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly zoomadbContext _context;
        private readonly IMapper _mapper;


        public OrdersController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrders() // XỬ LÝ GET ORDERS TRẢ VỀ KÈM CHUNG
                                                                           // VỚI DANH SÁCH CÁC ORDER DETAILS CỦA NÓ 
        {
          if (_context.Orders == null)
          {
              return NotFound();
          }

            var list = await _context.Orders.Include(o => o.OrderDetails).Include(o => o.User).Include(o => o.Transactions).Where(o => o.Status == 1).ToListAsync();

            //var list = await _context.Orders.ToListAsync();

            var orderDTOs = _mapper.Map<ICollection<OrderDTO>>(list);


            return Ok(orderDTOs);
         }
        // GET: api/Orders
        [HttpGet("unsuccess-orders")]

        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetUnSuccessOrders() // XỬ LÝ GET ORDERS TRẢ VỀ KÈM CHUNG
                                                                           // VỚI DANH SÁCH CÁC ORDER DETAILS CỦA NÓ 
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            try
            {
                var list = await _context.Orders.Include(o => o.OrderDetails).Include(o => o.User).Where(o => o.Status == 1).ToListAsync();

                var orderDTOs = _mapper.Map<ICollection<OrderDTO>>(list);
            return Ok(orderDTOs);

            }
            catch (Exception)
            {

                throw;
            }


        }


        [HttpGet("userId/{id}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersWithUserID(short id) // XỬ LÝ GET ORDERS TRẢ VỀ KÈM CHUNG VỚI USER

        {
            if (_context.Orders == null)
            {
                return NotFound();
            }

            var list = await _context.Orders.Where( o => o.UserId == id ).Include(o => o.OrderDetails).Include(o => o.Transactions).Include(o => o.User).ToListAsync();

            var orderDTOs = _mapper.Map<ICollection<OrderDTO>>(list);


            return Ok(orderDTOs);
        }


        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
          if (_context.Orders == null)
          {
              return NotFound();
          }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut]
        public async Task<IActionResult> PutOrder(OrderBody orderUpdate) // UPDATE ORDER INFORMATION
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == orderUpdate.Id);


            if (order != null)
            {
                order.Notes=orderUpdate.Notes;
                order.Status=orderUpdate.Status;
                _context.Entry(order).State = EntityState.Modified;

            }


            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(orderUpdate.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(_mapper.Map<Order>(order));
        }

        [HttpPut("order-details")]
        public async Task<IActionResult> PutOrderDetails(OrderDetailsBody update) // UPDATE ORDER DETAILS 
        {
            var orderDetail = await _context.OrderDetails.SingleOrDefaultAsync(o => o.Id == update.Id);

            if (orderDetail != null)
            {
                orderDetail.TicketDate = update.TicketDate;
                orderDetail.UsedTicket=update.UsedTicket;

                _context.Entry(orderDetail).State = EntityState.Modified;

            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderDetails.Any(o => o.Id == update.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(_mapper.Map<OrderDetailDTO>(orderDetail));
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
          if (_context.Orders == null)
          {
              return Problem("Entity set 'ZoomaContext.Orders'  is null.");
          }
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

public class OrderBody
{
    public int Id { get; set; }
    public string? Notes { get; set; }
    public byte Status { get; set; }
}

public class OrderDetailsBody
{
    public int Id { get; set; }
    public DateTime TicketDate { get; set; }
    //public byte Quantity { get; set; }
    public byte UsedTicket { get; set; }

}
