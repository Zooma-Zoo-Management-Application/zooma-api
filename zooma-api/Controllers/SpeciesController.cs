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
        [HttpGet()]
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
        public async Task<IActionResult> UpdateSpecies(int id, CreateSpecies species)
        {
            var speciesUpdate = await _context.Species.FindAsync(id);
            if (id != speciesUpdate.Id)
            {
                return BadRequest();
            }
            if (speciesUpdate != null)
            {
                speciesUpdate.Name = species.Name;
                speciesUpdate.Description = species.Description;
                speciesUpdate.ImageUrl = species.ImageUrl;
                speciesUpdate.TypeId = species.TypeId;
                _context.Entry(speciesUpdate).State = EntityState.Modified;
            }
            else
            {
                BadRequest("wrong id species");
            }
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
            return Ok(speciesUpdate);
        }

        // POST: api/Species
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Species>> PostSpecies(SpeciesBody species)
        {
            if (_context.Species == null)
            {
                return NotFound();
            }
            var speciesCreate = new Species
            {
                Name = species.Name,
                Description = species.Description,
                ImageUrl = species.ImageUrl,
                TypeId = species.TypeId
            };
            _context.Species.Add(speciesCreate);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetSpecies", new { id = speciesCreate.Id }, speciesCreate);

        }

        // DELETE
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
                return NotFound("Can't found the animal");
            }

            _context.Species.Remove(species);
            await _context.SaveChangesAsync();

            return Ok("Delete successfully");
        }

        private bool AnimalExists(int id)
        {
            return (_context.Animals?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        private bool SpeciesExists(int id)
        {
            return (_context.Species?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public class CreateSpecies
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? ImageUrl { get; set; }
            public int? TypeId { get; set; }
        }
        public class SpeciesBody
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
            public string? ImageUrl { get; set; }
            public int? TypeId { get; set; }

        }
    }
}
