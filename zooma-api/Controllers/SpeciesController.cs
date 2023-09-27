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
    public class SpeciesController : ControllerBase
    {
        public ZoomaContext _context = new ZoomaContext();
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public SpeciesController(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        //Hàm lấy tất cả species
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<SpeciesDTO>>> GetAllSpecies()
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var speciesDTO = _mapper.Map<ICollection<SpeciesDTO>>(await _context.Species.ToListAsync());

            return Ok(speciesDTO);
        }

        //Hàm tạo species mới
        [HttpPost("CreateSpecies")]
        public async Task<ActionResult<SpeciesDTO>> CreateSpecies(CreateSpecies createSpecies)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var species = _mapper.Map<Species>(createSpecies);
            _context.Species.Add(species);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecies", new { id = species.Id }, species);
        }
        //Hàm sửa species
        [HttpPut("UpdateSpecies/{id}")]
        public async Task<IActionResult> UpdateSpecies(int id, SpeciesDTO speciesDTO)
        {
            if (id != speciesDTO.Id)
            {
                return BadRequest();
            }
            var species = _mapper.Map<Species>(speciesDTO);
            _context.Entry(species).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Update success");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(id))
                {
                    return NotFound();
                    Console.WriteLine("Not found");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }   



        // GET: api/Species
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SpeciesDTO>>> GetSpecies()
        {
            var species = await _context.Species.ToListAsync();

            var specieDTOs = _mapper.Map<List<SpeciesDTO>>(species);

            if (specieDTOs == null || specieDTOs.Count == 0)
            {
                return NotFound("No species found.");
            }

            return specieDTOs;
        }

        // GET: api/Species/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Species>> GetSpecies(int id)
        {
          if (_context.Species == null)
          {
              return NotFound();
          }
            var species = await _context.Species.FindAsync(id);

            if (species == null)
            {
                return NotFound();
            }

            return species;
        }

        // PUT: api/Species/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutSpecies(int id, Species species)
        {
            if (id != species.Id)
            {
                return BadRequest();
            }

            _context.Entry(species).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SpeciesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Species
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Species>> PostSpecies(Species species)
        {
          if (_context.Species == null)
          {
              return Problem("Entity set 'ZoomaContext.Species'  is null.");
          }
            _context.Species.Add(species);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecies", new { id = species.Id }, species);
        }

        // DELETE: api/Species/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSpecies(int id)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var species = await _context.Species.FindAsync(id);
            if (species == null)
            {
                return NotFound();
            }

            _context.Species.Remove(species);
            await _context.SaveChangesAsync();

            return NoContent();
        }
  
        private bool SpeciesExists(int id)
        {
            return (_context.Species?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
