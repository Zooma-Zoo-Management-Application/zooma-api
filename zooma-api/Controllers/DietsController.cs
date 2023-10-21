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
        // GET: api/Diets/5
        [HttpGet("GetDietByName/{name}")]
        public async Task<ActionResult<Diet>> GetDietByName(string name)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            var diet = await _context.Diets.SingleOrDefaultAsync(d => d.Name == name);

            if (diet == null)
            {
                return NotFound();
            }

            return diet;
        }

        // PUT: api/Diets/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("UpdateDiet/{id}")]
        public async Task<IActionResult> PutDiet(int id, DietUpdate diet)
        {
            var dietUpdate = await _context.Diets.FindAsync(id);
            if (dietUpdate == null)
            {
                return BadRequest();
            }
            dietUpdate.Name = dietUpdate.Name ?? diet.Name;
            dietUpdate.Description = dietUpdate.Description ?? diet.Description;
            dietUpdate.CreateAt = dietUpdate.CreateAt;
            dietUpdate.UpdateAt = dietUpdate.UpdateAt;
            dietUpdate.ScheduleAt = dietUpdate.ScheduleAt;
            dietUpdate.Status = dietUpdate.Status;
            dietUpdate.Goal = dietUpdate.Goal ?? diet.Goal;
            dietUpdate.EndAt = dietUpdate.EndAt;
            dietUpdate.TotalEnergyValue = dietUpdate.TotalEnergyValue;


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

            return Ok(dietUpdate);
        }

        // POST: api/Diets
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("CreateDiet")]
        public async Task<ActionResult<Diet>> PostDiet(DietUpdate dietUpdate)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            Diet diet = new Diet
            {
                Name = dietUpdate.Name,
                Description = dietUpdate.Description,
                CreateAt = dietUpdate.CreateAt,
                UpdateAt = dietUpdate.UpdateAt,
                ScheduleAt = dietUpdate.ScheduleAt,
                Status = dietUpdate.Status,
                Goal = dietUpdate.Goal,
                EndAt = dietUpdate.EndAt,
                TotalEnergyValue = dietUpdate.TotalEnergyValue
            };

            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiet", new { id = diet.Id }, diet);

        }

        private bool DietExists(int id)
        {
            return (_context.Diets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        public class DietUpdate
        {
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public DateTime ScheduleAt { get; set; }
            public bool Status { get; set; }
            public string Goal { get; set; } = null!;
            public DateTime EndAt { get; set; }
            public double TotalEnergyValue { get; set; }
        }
        public class DietBody
        {
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public DateTime ScheduleAt { get; set; }
            public bool Status { get; set; }
            public string Goal { get; set; } = null!;
            public DateTime EndAt { get; set; }
            public double TotalEnergyValue { get; set; }

        }

    }
}