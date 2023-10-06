using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Repositories;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
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
                        //Id = int.Parse(response.TransactionId),
                        Date = order.OrderDate,
                        AccountNumber = response.BanKTranNo,
                        TransactionToken = response.Token,
                        AmountOfMoney = order.TotalPrice * 23500,
                        Status = order.Status,
                        OrderId = int.Parse(response.OrderId),
                        TransactionNo=response.TransactionId
                    };

                    _context.Transactions.Add(transaction);
                    _context.SaveChanges();

                    return Ok(new { transaction = transaction });
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

                    //ListCart.Instance.ClearCart();
                }
            }
            catch (Exception)
            {
                return BadRequest("dead");
                throw;
            }
            var order = repository.GetOrdersById(orderId);

            OrderInfo orderInfo = new OrderInfo()
            {
                OrderId= order.Id,
                Amount=order.TotalPrice * 23500,
                OrderDesc = "Demo cart",
                CreatedDate= DateTime.Now,

            };

            return Ok(new { url = createPaymentUrl(orderInfo) });

        }



        private String createPaymentUrl(OrderInfo order)
        {
            VnPayLibrary vnpay = new VnPayLibrary();

            string vnp_Returnurl = _configuration["VnPayConfig:vnp_Returnurl"];
            var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];
            var vnp_Url = _configuration["VnPayConfig:vnp_Url"];
            var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((double)order.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(HttpContext)); // LẤY RA IP ADDRESS CỦA NGƯỜI GỬI
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return paymentUrl;
        }

        [HttpPost("refund-transaction")]
        public async Task<IActionResult> RefundTransaction(RefundRequest refundRequest)
        {
            VnPayLibrary vnppay = new VnPayLibrary();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == refundRequest.OrderId);
            if (order == null)
            {
                return NotFound("order not found");
            }

            // Lấy thông tin transaction cần refund
            var transaction = await _context.Transactions
                                            .Where(t => t.OrderId == order.Id)
                                            .FirstOrDefaultAsync();

            if (transaction == null)
            {
                return NotFound("This order haven't paid or have transaction");
            }

            // ... (các bước kiểm tra và xác thực yêu cầu refund) ...

            var vnp_Api = _configuration["VnPayConfig:vnp_Api"];
            var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];
            var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];

            var vnp_RequestId = DateTime.Now.Ticks.ToString();
            var vnp_Version = VnPayLibrary.VERSION;
            var vnp_Command = "refund";
            var vnp_TransactionType = "02"; // Hoàn toàn. Dùng "03" cho hoàn một phần
            var vnp_Amount = transaction.AmountOfMoney * 100; // Chuyển về đơn vị tiền tệ nhỏ nhất
            var vnp_TxnRef = transaction.OrderId.ToString();
            var vnp_OrderInfo = refundRequest.Description;
            var vnp_TransactionNo = transaction.TransactionNo;
            var vnp_TransactionDate = transaction.Date.ToString("yyyyMMddHHmmss");
            var vnp_CreateDate = DateTime.Now.ToString("yyyyMMddHHmmss");
            var vnp_CreateBy = refundRequest.RefundBy ;
            var vnp_IpAddr = vnppay.GetIpAddress(HttpContext);

            var signData = vnp_RequestId + "|" + vnp_Version + "|" + vnp_Command + "|" + vnp_TmnCode + "|" + vnp_TransactionType + "|" + vnp_TxnRef + "|" + vnp_Amount + "|" + vnp_TransactionNo + "|" + vnp_TransactionDate + "|" + vnp_CreateBy + "|" + vnp_CreateDate + "|" + vnp_IpAddr + "|" + vnp_OrderInfo;
            var vnp_SecureHash = Utils.HmacSHA512(vnp_HashSecret, signData);

            var rfData = new
            {
                vnp_RequestId = vnp_RequestId,
                vnp_Version = vnp_Version,
                vnp_Command = vnp_Command,
                vnp_TmnCode = vnp_TmnCode,
                vnp_TransactionType = vnp_TransactionType,
                vnp_TxnRef = vnp_TxnRef,
                vnp_Amount = vnp_Amount,
                vnp_OrderInfo = vnp_OrderInfo,
                vnp_TransactionNo = vnp_TransactionNo,
                vnp_TransactionDate = vnp_TransactionDate,
                vnp_CreateBy = vnp_CreateBy,
                vnp_CreateDate = vnp_CreateDate,
                vnp_IpAddr = vnp_IpAddr,
                vnp_SecureHash = vnp_SecureHash

            };

            var jsonData = JsonSerializer.Serialize(rfData);

            try
            {
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(vnp_Api);
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";

                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write(jsonData);
                }
                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                var strData = "";
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    strData = streamReader.ReadToEnd();
                }
                var vnPayResponse = JsonSerializer.Deserialize<VnPayResponse>(strData);


                return Ok(new { VnPayResponse = vnPayResponse });


            }
            catch (WebException ex)
            {
                return BadRequest(new { Error = "Failed to connect to VnPay", Details = ex.Message });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Error = "Unexpected error", Details = ex.Message });
            }
        }




        // BODY CHO ITEM TRONG GIỎ HÀNG
        public class itemBody
        {
            public int ticketId { get; set; }
            public byte quantity { get; set; }
            public DateTime TicketDate { get; set; }
            public string? Description { get; set; }

        }

        public class RefundRequest
        {
            public int OrderId { get; set; }
            public string Description { get; set; }
            public string RefundBy { get; set; }
        }

        public class VnPayResponse
        {
            [JsonPropertyName("vnp_ResponseId")]
            public string ResponseId { get; set; }

            [JsonPropertyName("vnp_Command")]
            public string Command { get; set; }

            [JsonPropertyName("vnp_ResponseCode")]
            public string ResponseCode { get; set; }

            [JsonPropertyName("vnp_Message")]
            public string Message { get; set; }

            [JsonPropertyName("vnp_TmnCode")]
            public string TmnCode { get; set; }

            [JsonPropertyName("vnp_TxnRef")]
            public string TxnRef { get; set; }

            [JsonPropertyName("vnp_Amount")]
            public string Amount { get; set; }

            [JsonPropertyName("vnp_OrderInfo")]
            public string OrderInfo { get; set; }

            [JsonPropertyName("vnp_BankCode")]
            public string BankCode { get; set; }

            [JsonPropertyName("vnp_PayDate")]
            public string PayDate { get; set; }

            [JsonPropertyName("vnp_TransactionNo")]
            public string TransactionNo { get; set; }

            [JsonPropertyName("vnp_TransactionType")]
            public string TransactionType { get; set; }

            [JsonPropertyName("vnp_TransactionStatus")]
            public string TransactionStatus { get; set; }

            [JsonPropertyName("vnp_SecureHash")]
            public string SecureHash { get; set; }
        }




    }


}
