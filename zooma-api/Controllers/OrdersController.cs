using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;
using zooma_api.Repositories;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly zoomadbContext _context;
        private readonly IMapper _mapper;
        IOrderRepository _repo = new OrderRepository();

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
            var list = await _context.Orders.Include(o => o.OrderDetails).ThenInclude(o=>o.Ticket).Include(o => o.User).Include(o => o.Transactions).OrderByDescending(o => o.OrderDate).ToListAsync();

            //var list = await _context.Orders.ToListAsync();

            var orderDTOs = _mapper.Map<IEnumerable<OrderDTO>>(list);


            return Ok(orderDTOs);
         }
        // GET: api/Orders
        //[HttpGet("unsuccess-orders")]

        //public async Task<ActionResult<IEnumerable<OrderDTO>>> GetUnSuccessOrders() // XỬ LÝ GET ORDERS TRẢ VỀ KÈM CHUNG
        //                                                                   // VỚI DANH SÁCH CÁC ORDER DETAILS CỦA NÓ 
        //{
        //    if (_context.Orders == null)
        //    {
        //        return NotFound();
        //    }

        //    try
        //    {
        //        var list = await _context.Orders.Include(o => o.OrderDetails).Include(o => o.User).Where(o => o.Status == 1).ToListAsync();
        //        var orderDTOs = _mapper.Map<ICollection<OrderDTO>>(list);
        //    return Ok(orderDTOs);

        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //}


        [HttpGet("get-orders-by-user/{id}")]
        public async Task<ActionResult<IEnumerable<OrderDTO>>> GetOrdersWithUserID(short id) // XỬ LÝ GET ORDERS TRẢ VỀ KÈM CHUNG VỚI USER

        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            try
            {
                var list = _repo.GetOrdersByCustomerId(id);
                var orderDTOs = _mapper.Map<IEnumerable<OrderDTO>>(list);
                return Ok(orderDTOs);

            }
            catch (Exception)
            {

                throw;
            }
        }


        //[HttpGet("{id}")]
        //public async Task<ActionResult<Order>> GetOrder(int id)
        //{
        //  if (_context.Orders == null)
        //  {
        //      return NotFound();
        //  }
        //    var order = await _context.Orders.FindAsync(id);

        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    return order;
        //}



        // GET: api/Orders/5 ===================== CHECK TRẠNG THÁI ORDER ==========================
        [HttpGet("check-order-status/{id}")]
        public async Task<ActionResult<Order>> CheckOrderStatus(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == id);

            if (order == null)
            {
                return NotFound();
            }else
            {
                if(order.Status == 1)
                {
                    return Ok( new { msg = "Unpaid" , order = new { orderID = order.Id , status = order.Status} });
                }else if ( order.Status == 2)
                {
                    return Ok( new { msg = "Succesfully paid", order = new { orderID = order.Id, status = order.Status } });

                }else if ( order.Status == 3)
                {
                    return Ok(new { msg = "Refund sucessfully", order = new { orderID = order.Id, status = order.Status } });
                } else if (order.Status == 0)
                {
                    return Ok(new { msg = "Failed", order = new { orderID = order.Id, status = order.Status } });
                }

            }

            return order;
        }

        // PUT: api/Orders/5
        // =========================== UPDATE PAYMENTMETHOD ========================//
        [HttpPut("update-paymethod/{id}")]
        public async Task<IActionResult> UpdatePaymentMethod(int id, OrderBody orderUpdate) // UPDATE ORDER INFORMATION
        {
            var order = await _context.Orders.SingleOrDefaultAsync(o => o.Id == id);


            if (order != null)
            {
                if( order.Status != orderUpdate.Status) {
                    if (orderUpdate.Status == 1)
                    {
                        order.Status = orderUpdate.Status;
                        order.Notes = "Unpaid";
                        
                    }
                    else if (orderUpdate.Status == 2)
                    {
                        order.Status = orderUpdate.Status;
                        order.Notes = "Successfully paid";


                    }
                    else if (orderUpdate.Status == 3)
                    {
                        order.Status = orderUpdate.Status;
                        order.Notes = "Refund successfully";

                    }
                    else if (orderUpdate.Status == 0)
                    {
                        order.Status = orderUpdate.Status;
                        order.Notes = "Failed";
                    }
                }

                if (order.PaymentMethod != orderUpdate.PaymentMethod && orderUpdate.PaymentMethod != "")
                {
                    order.PaymentMethod = orderUpdate.PaymentMethod;

                }
                if (orderUpdate.Notes != "") order.Notes = orderUpdate.Notes;
                  //  order.Status = orderUpdate.Status;
                  //  _context.Entry(order).State = EntityState.Modified SAI LẦM TUỔI TRẺ 
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound(new { msg = "Order is not existed"});
                }
                else
                {
                    throw;
                }
            }

            return Ok(_mapper.Map<Order>(order));
        }

        [HttpPut("{id}/update-used-tickets")]
        public async Task<IActionResult> PutOrderDetails(int id,List<OrderDetailsBody> update) // UPDATE ORDER DETAILS 
        {
            var orderDetail = await _context.OrderDetails.Where(o => o.OrderId == id).Include(detail => detail.Ticket).ToListAsync();

            if (orderDetail != null)
            {
                if (update.Count == 0) return BadRequest("empty update details");
                foreach (var item in update)
                {
                    foreach (var detail in orderDetail)
                    {
                        if(item.Id == detail.Id)
                        {
                            //if(item.UsedTicket < detail.Quantity) // FE HANDLE
                            detail.UsedTicket = item.UsedTicket;
                            _context.Entry(detail).State = EntityState.Modified;
                            break;

                        }
                    }
                }
            }
            else
            {
                throw new Exception();
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.OrderDetails.Any(o => o.OrderId == id))
                {
                    return NotFound("Order details is not existed");
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { detailList = _mapper.Map<OrderDTO>(_repo.GetOrdersById(id)) , msg = "Updated successfully"});
        }

        //// POST: api/Orders
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPost]
        //public async Task<ActionResult<Order>> PostOrder(Order order)
        //{
        //  if (_context.Orders == null)
        //  {
        //      return Problem("Entity set 'ZoomaContext.Orders'  is null.");
        //  }
        //    _context.Orders.Add(order);
        //    await _context.SaveChangesAsync();

        //    return CreatedAtAction("GetOrder", new { id = order.Id }, order);
        //}

        //// DELETE: api/Orders/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> DeleteOrder(int id)
        //{
        //    if (_context.Orders == null)
        //    {
        //        return NotFound();
        //    }
        //    var order = await _context.Orders.FindAsync(id);
        //    if (order == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Orders.Remove(order);
        //    await _context.SaveChangesAsync();

        //    return NoContent();
        //}

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

public class OrderBody
{


    public string PaymentMethod { get; set; }
    public string? Notes { get; set; }

    public byte Status { get; set; }


}

public class OrderDetailsBody
{
    public int Id { get; set; }
    public byte UsedTicket { get; set; }


}
