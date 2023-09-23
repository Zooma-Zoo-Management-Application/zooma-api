using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using zooma_api.DTO;
using zooma_api.Models;
using static System.Net.Mime.MediaTypeNames;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnimalsController : ControllerBase
    {
        public ZoomaContext _context = new ZoomaContext();
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AnimalsController(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        // GET: api/Animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimals()
        {
            var animals = await _context.Animals.ToListAsync();

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            if (animalDTOs == null || animalDTOs.Count == 0)
            {
                return NotFound("No animals found.");
            }

            return animalDTOs;
        }

        // GET: api/Animals/5
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimalByName([FromQuery]string? name)
        {
          if (_context.Animals == null)
          {
                return NotFound();
          }

          if(string.IsNullOrEmpty(name))
          {
                return await _context.Animals.ToListAsync();
          } else
            {
                List<Animal> animals = await _context.Animals.Where(a => a.Name.Contains(name)).ToListAsync();

                if (animals.IsNullOrEmpty())
                {
                    return NotFound();
                }

                return animals;
            }
        }

        // PUT: api/Animals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
       /* [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimal(int id, Animal animal)
        {
            if (id != animal.Id)
            {
                return BadRequest();
            }

            _context.Entry(animal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalExists(id))
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

        // POST: api/Animals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Animal>> PostAnimal(Animal animal)
        {
          if (_context.Animals == null)
          {
              return Problem("Entity set 'ZoomaContext.Animals'  is null.");
          }
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimal", new { id = animal.Id }, animal);
        }

        // DELETE: api/Animals/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimal(int id)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }
            var animal = await _context.Animals.FindAsync(id);
            if (animal == null)
            {
                return NotFound();
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return NoContent();
        }
       */
        private bool AnimalExists(int id)
        {
            return (_context.Animals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
