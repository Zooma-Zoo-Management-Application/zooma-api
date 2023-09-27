using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost("createPaymentUrl")]
        public IActionResult CreatePaymentUrl([FromBody] OrderInfo order)
        {
            var vnpay = new VnPayLibrary();

            var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];
            var vnp_Url = _configuration["VnPayConfig:vnp_Url"];
            var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", "2.1.0");
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
            vnpay.AddRequestData("vnp_CreateDate", order.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString());
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", order.OrderDesc);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new { url = paymentUrl });
        }
    }
}
