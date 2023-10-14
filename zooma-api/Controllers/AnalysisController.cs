using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Xml.Schema;
using zooma_api.Repositories;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {

        private IAnalisticRepository _repository = new AnalisticRepository();

        [HttpGet("sixmonths-revenues")]
        public  IActionResult getRevenues()
        {
            var list = _repository.GetSixMonthsRevenues();
            double total = 0;

            foreach (var revenue in list)
            {
                total += revenue.Revenue;
            }

            return  Ok( new { total , list}); 
        }

        [HttpGet("sixmonths-tickets")]
        public  IActionResult getTickets()
        {
            var list = _repository.GetSixMonthsTicketQuantity();
            int totalTicket = 0;
            foreach (var ticket in list)
            {
                totalTicket += ticket.TotalTickets;
            }

            return Ok(new {totalTicket,list});
        }

        [HttpGet("inday-analysis")]

        public IActionResult GetIndayInformations()
        {
            return Ok(new { revenue = _repository.RevenuesInDay, quantityDetail = _repository.TicketsQuantityInDay });
        }


        [HttpGet("inday-revenues")]
        public IActionResult getRevenuesInday()
        {
            return Ok(new { revenue = _repository.RevenuesInDay });
        }

        [HttpGet("inday-tickets")]
        public IActionResult getTicketsInday()
        {
            return Ok( new { quantityDetail = _repository.TicketsQuantityInDay });
        }

    }
}
