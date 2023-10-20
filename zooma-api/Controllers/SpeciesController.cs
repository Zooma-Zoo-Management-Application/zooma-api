using AutoMapper;
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
        public zoomadbContext _context = new zoomadbContext();
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public SpeciesController(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }
        // GET: api/Species
        [HttpGet("GetAllSpecies")]
        public async Task<ActionResult<IEnumerable<SpeciesDTO>>> GetAllSpecies()
        {
            var species = await _context.Species.ToListAsync();
            if (species == null)
            {
                return NotFound();
            }
            var speciesDTO = _context.Species.Select(s => new SpeciesDTO
            {
                Id = s.Id,
                Name = s.Name,
                Description = s.Description,
                ImageUrl = s.ImageUrl,
                Status = s.Status,
                TypeId = s.TypeId
            });
            return Ok(speciesDTO);
        }

        // GET: api/Species/5
        [HttpGet("GetSpeciesById/{id}")]
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
        [HttpPut("UpdateSpecies/{id}")]
        public async Task<IActionResult> UpdateSpecies(int id, CreateSpecies species)
        {
            var speciesUpdate = await _context.Species.FindAsync(id);
            if (id != species.Id)
            {
                return BadRequest();
            }
            if (speciesUpdate == null)
            {
                return NotFound();
            }
            speciesUpdate.Name = speciesUpdate.Name ?? species.Name;
            speciesUpdate.Description = speciesUpdate.Description ?? species.Description;
            speciesUpdate.ImageUrl = speciesUpdate.ImageUrl ?? species.ImageUrl;
            speciesUpdate.Status = species.Status;
            speciesUpdate.TypeId = speciesUpdate.TypeId ?? species.TypeId;
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
        public async Task<ActionResult<Species>> PostSpecies(CreateSpecies species)
        {
            if (_context.Species == null)
            {
                return BadRequest("Species already exists.");
            }
            Species newSpecies = new Species()
            {
                Name = species.Name,
                Description = species.Description,
                ImageUrl = species.ImageUrl,
                Status = species.Status,
                TypeId = species.TypeId
            };
            _context.Species.Add(newSpecies);
            await _context.SaveChangesAsync();


            return CreatedAtAction("GetSpecies", new { id = species.Id }, species);
        }

        private bool SpeciesExists(int id)
        {
            return (_context.Species?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public class CreateSpecies
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? ImageUrl { get; set; }
            public bool Status { get; set; }
            public int? TypeId { get; set; }
        }
    }
}
