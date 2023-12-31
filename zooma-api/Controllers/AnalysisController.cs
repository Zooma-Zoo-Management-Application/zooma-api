﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Schema;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;
using zooma_api.Repositories;

namespace zooma_api.Controllers
{
    [Route("api/analysis")]
    [ApiController]
    public class AnalysisController : ControllerBase
    {

        private IAnalisticRepository _repository = new AnalisticRepository();
        private IOrderRepository repo = new OrderRepository();
        private IUserRepository user_repo = new UserRepository();


        private readonly zoomadbContext _context;
        private readonly IMapper _mapper;

        public AnalysisController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("sixmonths-analysis")]
        public  async Task<IActionResult> getRevenues()
        {
            double total = 0;
            int totalTicket = 0;
            var Revenuelist = _repository.GetSixMonthsRevenues();
            var orderDTOs = _mapper.Map<List<OrderDTO>>(repo.GetFiveRecentOrders());
            var Ticketlist = _repository.GetSixMonthsTicketQuantity();
            AllUsersQuantity UsersQuantity = await user_repo.GetUsersQuantityAsync();
            int quantity =  repo.GetQuantityOfSuccessOrders();



            foreach (var revenue in Revenuelist)
            {
                total += revenue.Revenue;
            }

            foreach( var ticket in Ticketlist)
            {
                totalTicket += ticket.TotalMonth;
            }

            return  Ok( new { TotalRevenue = total , TotalTickets = totalTicket, TotalSuccessOrders = quantity, Revenue = Revenuelist, Tickets = Ticketlist, RecentOrders =orderDTOs , UsersQuantity = UsersQuantity }); 
        }

        //[HttpGet("recent-orders")]
        //public async Task<ActionResult<IEnumerable<OrderDTO>>> GetRecentOrders()
        //{

        //    var orderDTOs = _mapper.Map<List<OrderDTO>>(repo.GetFiveRecentOrders());
        //    return Ok(orderDTOs);
        //}


        //[HttpGet("sixmonths-tickets")]
        //public  IActionResult getTickets()
        //{
        //    var list = _repository.GetSixMonthsTicketQuantity();
        //    int totalTicket = 0;
        //    foreach (var ticket in list)
        //    {
        //        totalTicket += ticket.TotalTickets;
        //    }

        //    return Ok(new {totalTicket,list});
        //}

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
