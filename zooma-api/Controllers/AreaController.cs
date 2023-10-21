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
            var areas = await _context.Areas.ToListAsync();

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
            var area = await _context.Areas.
                    FirstOrDefaultAsync(n => n.Id == id);
            if (area == null)
            {
                return NotFound("No area having that ID");
            }
            return Ok(area);
        }
        //update area
        [HttpPut("UpdateArea/{id}")]
        public async Task<IActionResult> UpdateArea(int id, AreaUpdate areaUpdate)
        {
            var area = await _context.Areas.FindAsync(id);
            if (id != area.Id)
            {
                return BadRequest();
            }
            area.Name = area.Name ?? areaUpdate.Name;
            area.Description = area.Description ?? areaUpdate.Description;

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
            public string Name { get; set; }
            public string Description { get; set; }
        }
    }
}