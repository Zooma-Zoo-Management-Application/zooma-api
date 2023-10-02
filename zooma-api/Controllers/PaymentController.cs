using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Repositories;
using VNPayDemo;
using zooma_api.DTO;
using zooma_api.Models;
using zooma_api.Repositories;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly ZoomaContext _context;
        private readonly IMapper _mapper;



        private IOrderRepository repository = new OrderRepository();


        public PaymentController(IConfiguration configuration, ZoomaContext context, IMapper mapper)
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;

        }

        [HttpPost("create-payment")]
        public IActionResult CreatePaymentUrl([FromBody] OrderInfo order)
        {
            VnPayLibrary vnpay = new VnPayLibrary();

            string vnp_Returnurl = _configuration["VnPayConfig:vnp_Returnurl"];
            var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];
            var vnp_Url = _configuration["VnPayConfig:vnp_Url"];
            var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(HttpContext)); // LẤY RA IP ADDRESS CỦA NGƯỜI GỬI
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new { url = paymentUrl });
        }


        [HttpGet]
        [Route("vnpay-return")] // API XỬ LÝ URL TRẢ VỀ KHI THANH TOÁN VNPAY XONG

        public IActionResult CreatePaymentUrl()
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(Request.Query, _configuration["VnPayConfig:vnp_HashSecret"]);

            if( response.Success )
            {
                var order = repository.GetOrdersById(int.Parse(response.OrderId));

                if( order != null )
                {
                    var transaction = new Transaction()
                    {
                        Id = int.Parse(response.TransactionId),
                        Date = order.OrderDate,
                        AccountNumber = response.BanKTranNo,
                        TransactionToken = response.Token,
                        AmountOfMoney = order.TotalPrice * 23500,
                        Status = order.Status,
                        OrderId = int.Parse(response.OrderId)
                    };

                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();

                    return Ok(new { transaction = response });
                }

            }
            
                return BadRequest("Transaction in VNPay has been failed");
            


        }

        // ============= DEMO GIỎ HÀNG VÀ THANH TOÁN LƯU ORDER VÀO DATABASE ============= //
   
        [HttpPost]
        [Route("add-to-cart")]

        public async Task<IActionResult> AddToCart(itemBody body) // ADD NHIỀU LẦN 
        {
            var ticket = await _context.Tickets.FindAsync(body.ticketId);


            try
            {

                if (ticket == null || body == null)
                {
                    return BadRequest("Please input valid item");
                }
                else
                {
                    var item = _mapper.Map<CartItemDTO>(ticket);

                    item.TicketDate = body.TicketDate;
                    item.quantity = body.quantity;
                    item.Description = body.Description;



                    ListCart.Instance.AddToCart(item);
                }
            }
            catch (Exception)
            {

                throw;
            }
            return Ok(ListCart.Instance.GetLists());

        }

        [HttpPost]
        [Route("checkout")]
        public IActionResult Checkout(short userID) // ADD XONG RỒI THÌ CHECKOUT
        {
            int orderId = 0;
            try
            {
                if(ListCart.Instance.GetLists() == null)
                {
                    throw new Exception("vl bro you didnt add the item in cart");
                }
                else
                {
                    orderId = repository.CreateOrder(userID, ListCart.Instance.GetLists());

                    ListCart.Instance.ClearCart();
                }
            }
            catch (Exception)
            {
                return BadRequest("dead");
                throw;
            }
            return Ok(new { url = createPaymentUrl(repository.GetOrdersById(orderId)) });

        }



        private string createPaymentUrl(Order order) // METHOD TẠO URL TỪ ORDER ĐÃ THANH TOÁN
        {
            VnPayLibrary vnpay = new VnPayLibrary();

            string vnp_Returnurl = _configuration["VnPayConfig:vnp_Returnurl"];
            var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];
            var vnp_Url = _configuration["VnPayConfig:vnp_Url"];
            var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.TotalPrice * 100 * 23500).ToString()); // TIỀN DOLLAR 🐧
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(HttpContext)); // LẤY RA IP ADDRESS CỦA NGƯỜI GỬI
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.Id);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.Id.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;

        }

        // BODY CHO ITEM TRONG GIỎ HÀNG
        public class itemBody
        {
            public int ticketId { get; set; }
            public byte quantity { get; set; }
            public DateTime TicketDate { get; set; }
            public string? Description { get; set; }



        }




    }


}
