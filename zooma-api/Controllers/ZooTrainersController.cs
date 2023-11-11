using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ZooTrainersController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;
        private readonly IZooTrainerRepository _zooTrainerRepository;

        public ZooTrainersController(zoomadbContext context, IMapper mapper, IZooTrainerRepository zooTrainerRepository)
        {
            _context = context;
            _mapper = mapper;
            _zooTrainerRepository = zooTrainerRepository;
        }

        // GET: api/ZooTrainers
        [HttpGet]
        public ActionResult<IEnumerable<UserDTO>> GetAllZooTrainers()
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            
            var zooTrainers = _zooTrainerRepository.GetAllZooTrainers();

            var zooTrainersDTO = _mapper.Map<List<UserDTO>>(zooTrainers);

            return zooTrainersDTO;
        }

        // Hàm lấy zootrainer với id

        [HttpGet("{id}/get-zootrainer-by-id")]
        public ActionResult<UserDTO> GetZooTrainerById(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var zootrainer = _zooTrainerRepository.GetZooTrainerById(id);

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
