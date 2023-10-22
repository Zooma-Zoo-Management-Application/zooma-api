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
    public class FoodSpeciesController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public FoodSpeciesController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm lấy tất cả foodSpecies ra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<FoodSpecyDTO>>> GetFoodSpecies()
        {
            if (_context.FoodSpecies == null)
            {
                return NotFound();
            }

            var foodSpecy = await _context.FoodSpecies.
                                                       Include(e => e.Species).
                                                       Include(e => e.Food).
                                                       ToListAsync();

            var foodSpecyDTO = _mapper.Map<List<FoodSpecyDTO>>(foodSpecy);

            return foodSpecyDTO;
        }

        // Hàm lấy foodSpecy dựa trên Id
        [HttpGet("{id}")]
        public async Task<ActionResult<FoodSpecyDTO>> GetFoodSpecyById(int id)
        {
            if (_context.FoodSpecies == null)
            {
                return NotFound();
            }

            var foodSpecy = await _context.FoodSpecies.
                                                       Include(e => e.Species).
                                                       Include(e => e.Food).
                                                       FirstOrDefaultAsync(e => e.Id == id);

            if (foodSpecy == null)
            {
                return NotFound("Can't found the food of this species");
            }

            var foodSpecyDTO = _mapper.Map<FoodSpecyDTO>(foodSpecy);

            return foodSpecyDTO;
        }

        // Hàm Update Food Specy
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFoodSpecy(int id, FoodSpecyUpdate foodSpecyUpdate)
        {
            var foodSpecy = await _context.FoodSpecies.FirstOrDefaultAsync(e => e.Id == id);

            if(foodSpecy == null)
            {
                return NotFound("Can't found the food of this specy");
            }

            foodSpecy.CompatibilityStatus = foodSpecyUpdate.CompatibilityStatus;

            _context.Entry(foodSpecy).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodSpecyExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update successfully!");
        }

        // Hàm tạo FoodSpecy
        [HttpPost]
        public async Task<ActionResult<FoodSpecyDTO>> CreateFoodSpecy(FoodSpecyCreate foodSpecyCreate)
        {
            if (_context.FoodSpecies == null)
            {
            return Problem("Entity set 'zoomadbContext.FoodSpecies'  is null.");
            }

            FoodSpecy foodSpecy = new FoodSpecy
            {
                FoodId = foodSpecyCreate.FoodId,
                SpeciesId = foodSpecyCreate.SpeciesId,
                CompatibilityStatus = foodSpecyCreate.CompatibilityStatus
            };

            var foodSpecyExists = _context.FoodSpecies.FirstOrDefault(e => e.FoodId ==  foodSpecyCreate.FoodId 
                                                                        && e.SpeciesId == foodSpecyCreate.SpeciesId);

            if(foodSpecyExists != null)
            {
                return BadRequest("This species food has existed!");
            }

            _context.FoodSpecies.Add(foodSpecy);
            await _context.SaveChangesAsync();

            return Ok(new { foodSpecyDTO = _mapper.Map<FoodSpecyDTO>(foodSpecy), message = "Create successfully!"});
        }

        // Hàm xóa specy dựa trên id
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFoodSpecy(int id)
        {
            if (_context.FoodSpecies == null)
            {
                return NotFound();
            }
            var foodSpecy = await _context.FoodSpecies.FindAsync(id);
            if (foodSpecy == null)
            {
                return NotFound();
            }

            _context.FoodSpecies.Remove(foodSpecy);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodSpecyExists(int id)
        {
            return (_context.FoodSpecies?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class FoodSpecyCreate
    {
        public int SpeciesId { get; set; }
        public int FoodId { get; set; }
        public bool CompatibilityStatus { get; set; }
    }

    public class FoodSpecyUpdate
    {
        public bool CompatibilityStatus { get; set; }
    }
}
