using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/species")]
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

        /// <summary>
        /// Return species by area
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET: api/Species
        [HttpGet("species/{id}")]
        public async Task<ActionResult<IEnumerable<Species>>> GetSpeciesInArea(int id)
        {
            // CAGES LÀM DB VÀ CODE CỨNG THÌ KHÔNG CẦN PHẢI CHECK LỖI, CHECK CAGE LÀ ĐƯỢC
            try
            {
                var cages = _context.Cages.Where(a => a.AreaId == id).Select(a => a.Id).ToList();

                if (cages.Count == 0 || cages == null)
                {
                    return NotFound(new { msg = "No cages have founded" });

                }
                //EAGER ONLY SPECIES FOR BETTER SPEED
                var animals = await _context.Animals.Where(a => cages.Contains((short)a.CageId)).Include(n => n.Species).ToListAsync();

                if (animals.Count == 0)
                {
                    return BadRequest(new { msg = "This area dont have any animals in cages" });
                }

                if (animals != null)
                {
                    var species = animals.Select(a => a.Species).DistinctBy(x => x.Id).ToList(); // LINQ DISTINCT                 
                    return Ok(new { species = _mapper.Map<List<SpeciesDTO>>(species) });
                }
                else
                {
                    return NotFound(new { msg = "No species have founded" });
                }
            }
            catch (Exception)
            {
                return BadRequest(new { msg = "Nhìn quanh lần cuối" });
            }



        }
        /// <summary>
        /// Return species by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
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
            var speciesExist = _context.Species.FirstOrDefault(e => e.Name == species.Name && e.Name != speciesUpdate.Name);

            if (speciesExist != null) {
                return BadRequest("This species has already existed before");
            }
            else if(speciesUpdate != null)
            {
                speciesUpdate.Name = species.Name;
                speciesUpdate.Description = species.Description;
                speciesUpdate.ImageUrl = species.ImageUrl;
                speciesUpdate.TypeId = species.TypeId;
                _context.Entry(speciesUpdate).State = EntityState.Modified;

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
            else
            {
                return BadRequest("wrong id species");
            }
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
            var speciesExist = _context.Species.FirstOrDefault(e => e.Name == species.Name);
            if (speciesExist != null)
            {
                return BadRequest("This species already existed before");
            }
            else
            {
                _context.Species.Add(speciesCreate);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSpecies", new { id = speciesCreate.Id }, speciesCreate);
            }
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
