using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class areaController : ControllerBase
    {
        private readonly zoomadbContext _context;

        public areaController(zoomadbContext context)
        {
            _context = context;
        }
        //get all
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<Area>>> GetAllAreas()
        {
            var areas = await _context.Areas.ToListAsync();

            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            return Ok(_context.Areas);
        }
        /// <summary>
        /// Return area by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //get area by id
        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> GetAreaById(short id)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            var area = await _context.Areas.
                    FirstOrDefaultAsync(n => n.Id == id);
            if (area == null)
            {
                return NotFound("No area having that ID");
            }
            return Ok(area);
        }
        /// <summary>
        /// Return a list of area have that species
        /// </summary>
        /// <param name="speciesId"></param>
        /// <returns></returns>
        //get area by speciesId
        [HttpGet("species/{speciesId}")]
        public async Task<IActionResult> GetAreaBySpeciesId(short speciesId)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }

            var species = await _context.Species.FirstOrDefaultAsync(n => n.Id == speciesId);

            if (species == null)
            {
                return NotFound("Can't found this species");
            }

            var cageOfSpecies = _context.Animals.Where(e => e.SpeciesId == speciesId).Select(e => e.CageId).ToList();

            var areaId = _context.Cages.Where(e => cageOfSpecies.Contains(e.AreaId)).Select(e => e.AreaId).Distinct().ToArray();


            if (areaId == null)
            {
                return NotFound("No area having that ID");
            }

            return Ok(areaId);
        }


        //update area
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, AreaUpdate areaUpdate)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(e => e.Id == id);
            if (area == null)
            {
                return NotFound("Can't found this area");
            }
            area.Name = areaUpdate.Name;
            area.Description = areaUpdate.Description;

            _context.Entry(area).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(area);
        }
        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }
        public class AreaUpdate
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }
}