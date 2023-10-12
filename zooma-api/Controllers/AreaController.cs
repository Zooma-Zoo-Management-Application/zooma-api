using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AreaController : ControllerBase
    {
        private readonly zoomadbContext _context;

        public AreaController(zoomadbContext context)
        {
            _context = context;
        }
        //get all
        [HttpGet("GetAllAreas")]
        public async Task<ActionResult<IEnumerable<Area>>> GetAllAreas()
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            return Ok(_context.Areas);
        }
        //get area by id
        [HttpGet("GetAreaById/{id}")]
        public async Task<ActionResult<Area>> GetAreaById(short id)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound("No area having that ID");
            }
            return Ok(area);
        }
        //create area
        [HttpPost("CreateArea")]
        public async Task<ActionResult<Area>> CreateArea(Area area)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'zoomadbContext.Areas'  is null.");
            }
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArea", new { id = area.Id }, area);
        }
        //update area
        [HttpPut("UpdateArea/{id}")]
        public async Task<IActionResult> UpdateArea(short id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest();
            }
            _context.Entry(area).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Update success");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
                {
                    return NotFound("Area not exists");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // GET: api/Area
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            if (_context.Areas == null)
            {
                return NotFound();
            }
            return await _context.Areas.ToListAsync();
        }

        // GET: api/Area/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Area>> GetArea(short id)
        {
            if (_context.Areas == null)
            {
                return NotFound();
            }
            var area = await _context.Areas.FindAsync(id);

            if (area == null)
            {
                return NotFound();
            }

            return area;
        }

        // PUT: api/Area/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutArea(short id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest();
            }

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

            return NoContent();
        }

        // POST: api/Area
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Area>> PostArea(Area area)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'zoomadbContext.Areas'  is null.");
            }
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArea", new { id = area.Id }, area);
        }

        // DELETE: api/Area/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArea(short id)
        {
            if (_context.Areas == null)
            {
                return NotFound();
            }
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }

            _context.Areas.Remove(area);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AreaExists(short id)
        {
            return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
