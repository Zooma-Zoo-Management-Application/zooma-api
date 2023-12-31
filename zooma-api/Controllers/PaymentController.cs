﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;
using VNPayDemo;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;
using zooma_api.Repositories;

namespace zooma_api.Controllers
{
    [Route("api/payment")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly zoomadbContext _context;
        private readonly IMapper _mapper;

        private IOrderRepository repository = new OrderRepository();


        public PaymentController(IConfiguration configuration, zoomadbContext context, IMapper mapper) // lmao
        {
            _configuration = configuration;
            _context = context;
            _mapper = mapper;

        }

        //[HttpPost("create-payment")]
        //public IActionResult CreatePaymentUrl([FromBody] OrderInfo order)
        //{
        //    VnPayLibrary vnpay = new VnPayLibrary();

        //    string vnp_Returnurl = _configuration["VnPayConfig:vnp_Returnurl"];
        //    var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];
        //    var vnp_Url = _configuration["VnPayConfig:vnp_Url"];
        //    var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];

        //    vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
        //    vnpay.AddRequestData("vnp_Command", "pay");
        //    vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
        //    vnpay.AddRequestData("vnp_Amount", (order.Amount * 100).ToString());
        //    vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
        //    vnpay.AddRequestData("vnp_CurrCode", "VND");
        //    vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(HttpContext)); // LẤY RA IP ADDRESS CỦA NGƯỜI GỬI
        //    vnpay.AddRequestData("vnp_Locale", "vn");
        //    vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + order.OrderId);
        //    vnpay.AddRequestData("vnp_OrderType", "other");
        //    vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
        //    vnpay.AddRequestData("vnp_TxnRef", order.OrderId.ToString());

        //    var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
        //    return Ok(new { url = paymentUrl });
        //}


        [HttpGet]
        [Route("vnpay-return")] // API XỬ LÝ URL TRẢ VỀ KHI THANH TOÁN VNPAY XONG
        public IActionResult returnUrlVnPay()
        {
            var pay = new VnPayLibrary();
            var response = pay.GetFullResponseData(Request.Query, _configuration["VnPayConfig:vnp_HashSecret"]);

            if (response.Success)
            {
                var order = repository.GetOrdersById(int.Parse(response.OrderId));

                if (response.VnPayResponseCode == "00")
                {
                    order.LastUpdateDate = DateTime.UtcNow.AddHours(7);
                    if (order != null)
                    {
                        var transaction = new Transaction()
                        {
                            //Id = int.Parse(response.TransactionId),
                            Date = order.OrderDate,
                            AccountNumber = response.BanKTranNo,
                            TransactionToken = response.Token,
                            AmountOfMoney = order.TotalPrice,
                            Status = true,
                            OrderId = int.Parse(response.OrderId),
                            TransactionNo = response.TransactionId
                        };

                        order.Status = 2; // THANH TOÁN THÀNH CÔNG
                        order.Notes = "Payment sucessfully";
                        _context.Entry(order).State = EntityState.Modified;

                        _context.Transactions.Add(transaction);
                        _context.SaveChanges();

                        return Ok(new { transaction = _mapper.Map<TransactionDTO>(transaction), Message = "Successful tracsaction" });
                    }
                }
                else
                {
                    order.Notes = "Unsuccessfull payment";
                    order.Status = 0; //THANH TOÁN FAILED
                    _context.Entry(order).State = EntityState.Modified;
                    _context.SaveChanges();
                    return BadRequest("Unsuccessfull payment, no transaction was created");     //hello

                }

            }

            return BadRequest("Transaction in VNPay has been failed");

        }

        // ============= DEMO GIỎ HÀNG VÀ THANH TOÁN LƯU ORDER VÀO DATABASE ============= //

        //[HttpPost]
        //[Route("add-to-cart")]

        //public async Task<IActionResult> AddToCart(itemBody body) // ADD NHIỀU LẦN 
        //{
        //    var ticket = await _context.Tickets.FindAsync(body.ticketId);


        //    try
        //    {

        //        if (ticket == null || body == null)
        //        {
        //            return BadRequest("Please input valid item");
        //        }
        //        else if ( body.quantity == 0)
        //        {
        //            return BadRequest("quantity have to be greater than 0");

        //        }
        //        else
        //        {
        //            var item = _mapper.Map<CartItemDTO>(ticket);

        //            //item.TicketDate = body.TicketDate;
        //            item.quantity = body.quantity;



        //            ListCart.Instance.AddToCart(item);
        //        }
        //    }
        //    catch (Exception)
        //    {

        //        throw;
        //    }
        //    return Ok(ListCart.Instance.GetLists());

        //}


        /// <summary>
        /// Checkout an order
        /// </summary>
        /// <param name="list" ></param>
        /// <param name="id" ></param>
        /// <returns></returns>
        [HttpPost]
        [Route("checkout/{id}")]
        public IActionResult Checkout(short id, List<CartItemDTO> list) // ADD XONG RỒI THÌ CHECKOUT
        {
            int orderId = 0;
            try
            {
                if(list.Count == 0 )
                {
                    throw new Exception("vl bro you didnt add the item in cart");
                }
                else
                {
                orderId = repository.CreateOrder(id, list);

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
                Amount=order.TotalPrice,
                OrderDesc = "Demo cart",
                CreatedDate= order.OrderDate,

            };

            Redirect(createPaymentUrl(orderInfo));

            return Ok(new { url = createPaymentUrl(orderInfo) , orderID = orderId });

        }

        //API THANH TOÁN LẠI TỪ ORDER ĐÃ CÓ

        /// <summary>
        /// Take an old unsucessful order and repay it or checkout the pending order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("repay/{orderId}")]
        public IActionResult repayUrl(int orderId )
        {
            var order = repository.GetOrdersById(orderId);
            if (order == null )
            {
                    return NotFound("order not found");
            }
            else
            {
                var orderDetails = repository.GetOrderDetailsByOrderId(orderId);
                var ticketDate = orderDetails[1].TicketDate;
                if(ticketDate < DateTime.Now)
                {
                    return BadRequest("Invalid ticket date, please try again");
                }
                if (order.Status == 2 || order.Status == 3)
                {
                    return BadRequest(new { msg = "can not repay this order" });
                }


            }
            int id = 0;
            if(order.Status ==1 )
            {
                id = orderId;
            }else if(order.Status == 0)
            {
                id = repository.RepayOrder(order.UserId, order);
            }

            VnPayLibrary vnpay = new VnPayLibrary();

            string vnp_Returnurl = _configuration["VnPayConfig:vnp_Returnurl"];
            var vnp_TmnCode = _configuration["VnPayConfig:vnp_TmnCode"];
            var vnp_Url = _configuration["VnPayConfig:vnp_Url"];
            var vnp_HashSecret = _configuration["VnPayConfig:vnp_HashSecret"];

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
            vnpay.AddRequestData("vnp_Amount", ((double)order.TotalPrice * 100).ToString()); // CHỖ NÀY ĐỂ FLOAT LÀ NÓ RA HEX 1.8E+..
            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", vnpay.GetIpAddress(HttpContext)); // LẤY RA IP ADDRESS CỦA NGƯỜI GỬI
            vnpay.AddRequestData("vnp_Locale", "vn");
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + id);
            vnpay.AddRequestData("vnp_OrderType", "other");
            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl);
            vnpay.AddRequestData("vnp_TxnRef", id.ToString());

            var paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);
            return Ok(new { url = paymentUrl , orderID = id });
        }

        // ===========XỬ LÝ PAYMENT URL KHI CHECK OUT=============//
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
            vnpay.AddRequestData("vnp_Amount", ((double)order.Amount * 100).ToString()); // CHỖ NÀY ĐỂ FLOAT LÀ NÓ RA HEX 1.8E+..
            vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss"));
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


        /// <summary>
        /// Refund order that have been paid
        /// </summary>
        /// <param name="refundRequest"></param>
        /// <returns></returns>
        [HttpPost("refund")]
        public async Task<IActionResult> RefundTransaction(RefundRequest refundRequest)
        {
            VnPayLibrary vnppay = new VnPayLibrary();

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == refundRequest.OrderId);
            if (order == null)
            {
                return NotFound("order not found");
            }
            else
            {
                var orderDetails = repository.GetOrderDetailsByOrderId(refundRequest.OrderId);
                var ticketDate = orderDetails[1].TicketDate;
                if (ticketDate < DateTime.Now)
                {
                    return BadRequest("Ihis order had been overdated, can not refund");
                }
                if (order.Status == 0 || order.Status == 1)
                {
                    return BadRequest(new { msg = "can not refund this order" });
                }
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

            var vnp_RequestId = DateTime.UtcNow.AddHours(7).Ticks.ToString();
            var vnp_Version = VnPayLibrary.VERSION;
            var vnp_Command = "refund";
            var vnp_TransactionType = "02"; // Hoàn toàn. Dùng "03" cho hoàn một phần
            var vnp_Amount = transaction.AmountOfMoney * 100; // Chuyển về đơn vị tiền tệ nhỏ nhất
            var vnp_TxnRef = transaction.OrderId.ToString();
            var vnp_OrderInfo = refundRequest.Description;
            var vnp_TransactionNo = transaction.TransactionNo;
            var vnp_TransactionDate = transaction.Date.ToString("yyyyMMddHHmmss");
            var vnp_CreateDate = DateTime.UtcNow.AddHours(7).ToString("yyyyMMddHHmmss");
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


                return Ok(new { VnPayResponse = vnPayResponse , Url = strData , Message = addRefundTransaction(vnPayResponse,transaction,order) });


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

        // =================XỬ LÝ DATABASE TẠO TRANSACTION REFUND  KHI RESPONSE TRẢ VỀ ==================//
        private string addRefundTransaction(VnPayResponse response, Transaction transaction, Order order) 
        {
            if(response.Message.Contains("Success"))
            {
                try
                {
                    var _transaction = new Transaction()
                    {
                        //Id = int.Parse(response.TransactionId),
                        Date = DateTime.ParseExact(response.PayDate, "yyyyMMddHHmmss", CultureInfo.InvariantCulture),
                        AccountNumber = response.ResponseId,
                        TransactionToken = transaction.TransactionToken,
                        AmountOfMoney = transaction.AmountOfMoney,
                        Status = true,
                        OrderId = transaction.OrderId,
                        TransactionNo = response.TransactionNo
                    };

                    _context.Transactions.Add(_transaction);
                    order.Status = 3; // REFUND THÀNH CÔNG
                    order.Notes = "Refund sucessfully";
                    _context.Entry(order).State = EntityState.Modified;
                    _context.SaveChanges();
                    repository.updateRefundOrder(transaction.OrderId);
                    return "Refund successfully";


                }
                catch (Exception)
                {

                    throw;
                }

            }
            else
            {
                return "This order have already refund";
            }


        }




        // BODY CHO ITEM TRONG GIỎ HÀNG
        public class itemBody
        {
            public int ticketId { get; set; }
            public byte quantity { get; set; }
            public DateTime TicketDate = DateTime.UtcNow.AddHours(7);

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
