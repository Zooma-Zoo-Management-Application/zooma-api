using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
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
    public class TypesController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public TypesController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // GET: api/Types
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TypeDTO>>> GetAllTypes()
        {
            if (_context.Types == null)
            {
                return NotFound();
            }

            var types = _context.Types.ToList();

            var typeDTO = _mapper.Map<List<TypeDTO>>(types);

            return typeDTO;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TypeDTO>> GetType(int id)
        {
            if (_context.Types == null)
            {
                return NotFound();
            }
            var type = await _context.Types.FindAsync(id);

            if (@type == null)
            {
                return NotFound("Can't found this type");
            }

            var typeDTO = _mapper.Map<TypeDTO>(type);

            return typeDTO;
        }

        // PUT: api/Types/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutType(int id, TypeUpdate typeUpdate)
        {
            var type = _context.Types.Find(id);

            if (type == null)
            {
                return NotFound("Can't found this type");
            }

            type.Name = typeUpdate.Name;
            type.Description = typeUpdate.Description;
            type.ImageUrl = typeUpdate.ImageUrl;
            type.Status = typeUpdate.Status;

         

            _context.Entry(type).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TypeExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update successfully");
        }

        private bool TypeExists(int id)
        {
            return (_context.Types?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class TypeUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool Status { get; set; }
    }
}