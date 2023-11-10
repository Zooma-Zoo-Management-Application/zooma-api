using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class foodsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;
        private readonly IFoodRepository _foodRepository;

        public foodsController(zoomadbContext context, IMapper mapper, IFoodRepository foodRepository)
        {
            _context = context;
            _mapper = mapper;
            _foodRepository = foodRepository;
        }

        // GET: api/Foods
        [HttpGet]
        public ActionResult<IEnumerable<FoodDTO>> GetFoods()
        {
            if (_context.Foods == null)
            { 
                return NotFound();
            }
            
            var food = _foodRepository.GetAllFoods();

            var foodDTO = _mapper.Map<List<FoodDTO>>(food);

            return foodDTO;
        }

        // GET: api/Foods/5
        [HttpGet("{id}")]
        public ActionResult<FoodDTO> GetFoodById(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = _foodRepository.GetById(id);

            if (food == null)
            {
                return NotFound("Can't found this food");
            }

            var foodDTO = _mapper.Map<FoodDTO>(food);

            return foodDTO;
        }

        // PUT: api/Foods/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateFood(int id, FoodUpdate foodUpdate)
        {

            var food = _context.Foods.FirstOrDefault(a => a.Id == id);

            if (food == null)
            {
                return NotFound("Can't found this food");
            }

            food.Description = foodUpdate.Description;
            food.EnergyValue = foodUpdate.EnergyValue;
            food.ImageUrl = foodUpdate.ImageUrl;
            food.Status = foodUpdate.Status;

            _context.Entry(food).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!FoodExists(id))
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

        // POST: api/Foods
        [HttpPost]
        public async Task<ActionResult<FoodDTO>> PostFood(FoodCreate foodCreate)
        {
            if (_context.Foods == null)
            {
              return Problem("Entity set 'zoomadbContext.Foods'  is null.");
            }

            Food food = new Food
            {
                Name = foodCreate.Name,
                Description = foodCreate.Description,
                EnergyValue = foodCreate.EnergyValue,
                ImageUrl = foodCreate.ImageUrl,
                Status = true
            };

            var existingFood = _context.Foods.FirstOrDefault(f => f.Name == food.Name);

            if (existingFood != null)
            {
                return BadRequest("This food has added before!");
            }

            _context.Foods.Add(food);
            await _context.SaveChangesAsync();

            return Ok(new { foodDTO = _mapper.Map<FoodDTO>(food), message = "Add food successfully!"});   
        }

        // DELETE: api/Foods/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteFood(int id)
        {
            if (_context.Foods == null)
            {
                return NotFound();
            }
            var food = await _context.Foods.FindAsync(id);
            if (food == null)
            {
                return NotFound();
            }

            _context.Foods.Remove(food);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool FoodExists(int id)
        {
            return (_context.Foods?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class FoodCreate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double EnergyValue { get; set; }
        public string? ImageUrl { get; set; }
    }
    public class FoodUpdate
    {
        public string? Description { get; set; }
        public double EnergyValue { get; set; }
        public string? ImageUrl { get; set; }
        public bool Status { get; set; }
    }
}
