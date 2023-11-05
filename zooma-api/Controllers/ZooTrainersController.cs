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
    public class ZooTrainersController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public ZooTrainersController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/ZooTrainers
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDTO>>> GetAllZooTrainers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            
            var zooTrainers = await _context.Users.Where(a => a.RoleId == 2).ToListAsync();

            var zooTrainersDTO = _mapper.Map<List<UserDTO>>(zooTrainers);

            return zooTrainersDTO;
        }

        // Hàm lấy zootrainer với id

        [HttpGet("{id}/get-zootrainer-by-id")]
        public async Task<ActionResult<UserDTO>> GetZooTrainerById(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var zootrainer = _context.Users.FirstOrDefault(a => a.Id == id);

            if (zootrainer == null)
            {
                return NotFound("Khong tim thay zoo trainer nay");
            }

            if (zootrainer.RoleId != 2)
            {
                return BadRequest("Nguoi nay khong phai zoo trainer");
            }

            var zooTrainerDTO = _mapper.Map<UserDTO>(zootrainer);

            return zooTrainerDTO;
        }

        private bool UserExists(short id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
