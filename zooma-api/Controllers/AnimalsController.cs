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
        public zoomadbContext _context = new zoomadbContext();
        private readonly IConfiguration _config;
        private readonly IMapper _mapper;

        public AnimalsController(IConfiguration config, IMapper mapper)
        {
            _config = config;
            _mapper = mapper;
        }

        // Ham lay tat ca animal
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimals()
        {
            var animals = await _context.Animals.
                Include(n => n.TrainingPlan).
                Include(n => n.Diet).
                Include(n => n.Cage).
                Include(n => n.Species).
                ToListAsync();

            //     var animalss = await _context.Animals.Join(_context.Cages, x => x.CageId, y => y.Id,
            //         (x, y) => new { x, y }).ToListAsync();

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

                if (animalDTOs == null || animalDTOs.Count == 0)
                {
                    return NotFound("No animals found.");
                }

                return animalDTOs;
        }

        // ham lay animal dua tren Id
        [HttpGet("{id}")]
        public async Task<ActionResult<AnimalDTO>> GetAnimalById(int id)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var animal = await _context.Animals.
                                                Include(n => n.TrainingPlan).
                                                Include(n => n.Diet).
                                                Include(n => n.Species).
                                                Include(n => n.Cage).
                                                FirstOrDefaultAsync(e => e.Id == id);

            if (animal == null)
            {
                return NotFound();
            }

            var animalDTO = _mapper.Map<AnimalDTO>(animal);

            return animalDTO;
        }

        // Ham lay animal dua tren name
        [HttpGet("name")]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimalByName([FromQuery] string? name)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            List<Animal> animals = new List<Animal>();

            List<AnimalDTO> animalsDTO;

            if (string.IsNullOrEmpty(name))
            {
                animals = await _context.Animals.ToListAsync();
                animalsDTO = _mapper.Map<List<AnimalDTO>>(animals);
                return animalsDTO;
            }
            else
            {
                animals = await _context.Animals.Where(a => a.Name.Contains(name)).
                                                                                    Include(n => n.TrainingPlan).
                                                                                    Include(n => n.Diet).
                                                                                    Include(n => n.Species).
                                                                                    Include(n => n.Cage).ToListAsync();

                animalsDTO = _mapper.Map<List<AnimalDTO>>(animals);

                if (animalsDTO.IsNullOrEmpty())
                {
                    return NotFound("We can't found the animal that match your search");
                }

                return animalsDTO;
            }
        }

        // Ham update thong tin cua animal
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimalDetails(int id, [FromBody] AnimalUpdate animalUpdate)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == id);

            if (animal == null)
            {
                return NotFound("NOT FOUND");
            }

            _mapper.Map(animal, animalUpdate);

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

            return Ok("Update succesfully");
                
        }

        // POST: api/Animals
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AnimalUpdate>> AddAnimal(AnimalUpdate animalDTO)
        {
            if (_context.Animals == null)
            {
                return Problem("Entity set 'ZoomaContext.Animals'  is null.");
            }
            var animal = _mapper.Map<Animal>(animalDTO);
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAnimalById", new { id = animal.Id }, animal);
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
                return NotFound("Can't found the animal");
            }

            _context.Animals.Remove(animal);
            await _context.SaveChangesAsync();

            return Ok("Create successfully");
        }

        private bool AnimalExists(int id)
        {
            return (_context.Animals?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
