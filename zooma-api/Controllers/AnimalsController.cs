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
    public class AnimalsController : ControllerBase
    {
        private readonly ZooMaContext _context =  new ZooMaContext();
        private readonly IMapper _mapper;

        public AnimalsController(IMapper mapper)
        {
            _mapper = mapper;
        } 

        // GET: api/Animals
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimals()
        {

            var animals = await _context.Animals.ToListAsync();

            if(animals.Count == 0)
             {
                return Ok(new List<Animal>());
             }

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return Ok(animalDTOs);
        }

        // GET: api/Animals/5
        [HttpGet("{text}")]
        public async Task<ActionResult<IEnumerable<Animal>>> GetAnimalByName(string text)
        {
          if (_context.Animals == null)
          {
              return NotFound();
          }
            List<Animal> animals = await _context.Animals.Where(a => a.Name.Contains(text)).ToListAsync();

            if (animals.Count == 0)
            {
                return NotFound();
            }

            return animals;
        }

        // PUT: api/Animals/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
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
              return Problem("Entity set 'ZooMaContext.Animals'  is null.");
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

        private bool AnimalExists(int id)
        {
            return (_context.Animals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
