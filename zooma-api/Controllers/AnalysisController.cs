using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
            return  Ok( _repository.GetGetSixMonthsRevenues()); 
        }

        [HttpGet("sixmonths-tickets")]
        public  IActionResult getTickets()
        {
            return Ok(_repository.GetGetSixMonthsTicketQuantity());
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
