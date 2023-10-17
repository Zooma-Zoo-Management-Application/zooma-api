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

        // ham lay animal dua tren CageId
        [HttpGet("/get-animals-by-cageId/{id}")]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimalsByCageId(int id)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var cages = _context.Cages.FirstOrDefault(a => a.Id == id);

            if (cages == null)
            {
                return NotFound("Cage Not Found!!");
            }

            var animals = _context.Animals.Where(c => c.CageId == cages.Id).
                                                                           Include(n => n.TrainingPlan).
                                                                           Include(n => n.Diet).
                                                                           Include(n => n.Species).
                                                                           Include(n => n.Cage).
                                                                           ToList();

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return animalDTOs;
        }

        // ham lay animal dua tren AreaId
        [HttpGet("/get-animals-by-areaId/{id}")]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimalsByAreaId(int id)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var areaId = _context.Areas.FirstOrDefault(a => a.Id == id);

            if (areaId == null)
            {
                return NotFound("Khong tim thay area nay");
            }

            var cages = _context.Cages.Where(a => a.AreaId == areaId.Id).Select(a => a.Id).ToList();

            if (cages.Count == 0 || cages == null)
            {
                return NotFound("Khong tim thay cage trong area nay");
            }

            var animals = _context.Animals
                .Where(a => cages.Contains((short)a.CageId)).
                                                            Include(n => n.TrainingPlan).
                                                            Include(n => n.Diet).
                                                            Include(n => n.Species).
                                                            Include(n => n.Cage).
                                                            ToList();

            if (animals == null)
            {
                return NotFound();
            }

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return animalDTOs;
        }

        // Ham update thong tin cua animal
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAnimalDetails(int id, AnimalUpdate animalUpdate)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == id);

            if (animal == null)
            {
                return NotFound("NOT FOUND");
            }

            animal.Name = animal.Name ?? animalUpdate.Name;
            animal.Height = animal.Height ?? animalUpdate.Height;
            animal.Weight = animal.Weight ?? animalUpdate.Weight;
            animal.Description = animal.Description ?? animalUpdate.Description;
            animal.Status = animalUpdate.Status;
            animal.SpieciesId = animalUpdate.SpieciesId;
            animal.DietId = animalUpdate.DietId;
            animal.CageId = animal.CageId ?? animalUpdate.CageId;
            animal.TrainingPlanId = animalUpdate.TrainingPlanId;

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
        public async Task<ActionResult<AnimalDTO>> AddAnimal(AnimalUpdate animalDTO)
        {
            if (_context.Animals == null)
            {
                return Problem("Entity set 'ZoomaContext.Animals'  is null.");
            }

            Animal animal = new Animal
            {
                Name = animalDTO.Name,
                ArrivalDate = animalDTO.ArrivalDate,
                DateOfBirth = animalDTO.DateOfBirth,
                Height = animalDTO.Height,
                Weight = animalDTO.Weight,
                Description = animalDTO.Description,
                Status = animalDTO.Status,
                SpieciesId = animalDTO.SpieciesId,
                DietId = animalDTO.DietId,
                CageId = animalDTO.CageId,
                TrainingPlanId = animalDTO.TrainingPlanId,
            };

            //            var animal = _mapper.Map<Animal>(animalDTO);
            _context.Animals.Add(animal);
            await _context.SaveChangesAsync();

            return Ok(new { animalDTO = _mapper.Map<AnimalDTO>(animal), message = "Animal created successfully" });
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

            return Ok("Delete successfully");
        }

        private bool AnimalExists(int id)
        {
            return (_context.Animals?.Any(e => e.Id == id)).GetValueOrDefault();
        }

    }
    public class AnimalUpdate
    {
        public string? Name { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int SpieciesId { get; set; }
        public int? DietId { get; set; }
        public short? CageId { get; set; }
        public short? TrainingPlanId { get; set; }
    }

}
