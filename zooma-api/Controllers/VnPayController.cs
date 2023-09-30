using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using zooma_api.DTO;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public VnPayController(IConfiguration configuration)
        {
            _configuration = configuration;
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
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
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

        [Route("vnpay-return")]

        public IActionResult CreatePaymentUrl()
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(Request.Query, _configuration["VnPayConfig:vnp_HashSecret"]);

            return Ok(response);




        }




    }
}
