using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietController : ControllerBase
    {
        private readonly zoomadbContext _context;

        public DietController(zoomadbContext context)
        {
            _context = context;
        }
        //Get all diets in the system
        [HttpGet("GetAllDiets")]
        public async Task<ActionResult<IEnumerable<Diet>>> GetAllDiets()
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'ZoomaContext.Diets'  is null.");
            }
            return await _context.Diets.ToListAsync();
        }
        //Get detailed information of a diet
        [HttpGet("GetDietsByID/{id}")]
        public async Task<ActionResult<Diet>> GetDietByID(int id)
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'ZoomaContext.Diets'  is null.");
            }
            var diet = await _context.Diets.FindAsync(id);

            if (diet == null)
            {
                return NotFound();
            }

            return diet;
        }


        // Create a new diet
        [HttpPost("CreateDiets")]
        public async Task<ActionResult<Diet>> CreateDiet(Diet diet)
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'ZoomaContext.Diets'  is null.");
            }
            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiet", new { id = diet.Id }, diet);
        }
        //Update a diet
        [HttpPut("UpdateDiets/{id}")]
        public async Task<IActionResult> UpdateDiet(int id, Diet diet)
        {
            if (id != diet.Id)
            {
                return BadRequest("No diets with that ID");
            }

            _context.Entry(diet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!DietExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }

        // GET: api/Diet
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Diet>>> GetDiets()
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            return await _context.Diets.ToListAsync();
        }

        // GET: api/Diet/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Diet>> GetDiet(int id)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            var diet = await _context.Diets.FindAsync(id);

            if (diet == null)
            {
                return NotFound();
            }

            return diet;
        }

        // PUT: api/Diet/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiet(int id, Diet diet)
        {
            if (id != diet.Id)
            {
                return BadRequest();
            }

            _context.Entry(diet).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DietExists(id))
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

        // POST: api/Diet
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Diet>> PostDiet(Diet diet)
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'zoomadbContext.Diets'  is null.");
            }
            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiet", new { id = diet.Id }, diet);
        }

        // DELETE: api/Diet/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiet(int id)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            var diet = await _context.Diets.FindAsync(id);
            if (diet == null)
            {
                return NotFound();
            }

            _context.Diets.Remove(diet);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DietExists(int id)
        {
            return (_context.Diets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
