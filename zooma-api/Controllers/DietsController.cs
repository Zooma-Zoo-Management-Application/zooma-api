using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietsController : ControllerBase
    {
        private readonly zoomadbContext _context;

        public DietsController(zoomadbContext context)
        {
            _context = context;
        }

        // GET: api/Diets
        [HttpGet("GetDiets")]
        public async Task<ActionResult<IEnumerable<Diet>>> GetDiets()
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            return await _context.Diets.ToListAsync();
        }

        // GET: api/Diets/5
        [HttpGet("GetDietByID/{id}")]
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

        // PUT: api/Diets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateDiet/{id}")]
        public async Task<IActionResult> PutDiet(int id, string name, string? Description, DateTime CreateAt, DateTime EndAt, bool status, string Goal, DateTime UpdateAt, DateTime ScheduleAt, double TotalEnergyValue)
        {
            var dietUpdate = await _context.Diets.SingleOrDefaultAsync(d => d.Id == id);
            if (dietUpdate == null)
            {
                return NotFound();
            }
            try
            {
                dietUpdate.Name = name;
                dietUpdate.Description = Description;
                dietUpdate.CreateAt = CreateAt;
                dietUpdate.EndAt = EndAt;
                dietUpdate.Status = status;
                dietUpdate.Goal = Goal;
                dietUpdate.UpdateAt = UpdateAt;
                dietUpdate.ScheduleAt = ScheduleAt;
                dietUpdate.TotalEnergyValue = TotalEnergyValue;

                _context.Diets.Update(dietUpdate);
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

        // POST: api/Diets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateDiet")]
        public async Task<ActionResult<Diet>> PostDiet(string name, string? Description, DateTime CreateAt, DateTime EndAt, bool status, string Goal, DateTime UpdateAt, DateTime ScheduleAt, double TotalEnergyValue)
        {
            var diet = new Diet
            {
                Name = name,
                Description = Description,
                CreateAt = CreateAt,
                EndAt = EndAt,
                Status = status,
                Goal = Goal,
                UpdateAt = UpdateAt,
                ScheduleAt = ScheduleAt,
                TotalEnergyValue = TotalEnergyValue


            };

            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiet", new { id = diet.Id }, diet);

        }
        private bool DietExists(int id)
        {
            return (_context.Diets?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
}

