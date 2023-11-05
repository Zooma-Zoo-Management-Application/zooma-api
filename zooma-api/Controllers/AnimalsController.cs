using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SQLitePCL;
using zooma_api.DTO;
using zooma_api.Interfaces;
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
        private readonly IAnimalRepository _animalRepository;

        public AnimalsController(IConfiguration config, IMapper mapper, IAnimalRepository animalRepository)
        {
            _config = config;
            _mapper = mapper;
            _animalRepository = animalRepository;
        }

        // Ham lay tat ca animal
        [HttpGet]
        public ActionResult<IEnumerable<AnimalDTO>> GetAnimals()
        {
            var animals = _animalRepository.GetAllAnimals();

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            if (animalDTOs == null || animalDTOs.Count == 0)
            {
                return NotFound("No animals found.");
            }

            return animalDTOs;
        }

        // ham lay animal dua tren Id
        [HttpGet("{id}")]
        public ActionResult<AnimalDTO> GetAnimalById(int id)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var animal = _animalRepository.GetAnimalById(id); 

            if (animal == null)
            {
                return NotFound("Can't found this animal");
            }

            var animalDTO = _mapper.Map<AnimalDTO>(animal);

            return animalDTO;
        }

        // Ham lay animal dua tren name
        [HttpGet("name")]
        public ActionResult<IEnumerable<AnimalDTO>> GetAnimalByName(string? name)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var animals = _animalRepository.GetAnimalByName(name);

            var animalsDTO = _mapper.Map<List<AnimalDTO>>(animals);

            return animalsDTO;
        }

        // ham lay animal dua tren CageId
        [HttpGet("{id}/get-animals-by-cageId")]
        public ActionResult<IEnumerable<AnimalDTO>> GetAnimalsByCageId(int id)
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

            var animals = _animalRepository.GetAnimalsByCageId(id);

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return animalDTOs;
        }

        // ham lay animal dua tren AreaId
        [HttpGet("{id}/get-animals-by-areaId")]
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

            var animals = _animalRepository.GetAnimalsByAreaId(id);

            if (animals == null)
            {
                return NotFound();
            }

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return animalDTOs;
        }

        // ham lay animal dua tren CageId = null
        [HttpGet("animal-without-cage")]
        public ActionResult<IEnumerable<AnimalDTO>> GetAllAnimalNoCage()
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var animal = _animalRepository.GetAllAnimalsWithNoCage();

            if (animal == null)
            {
                return NotFound("Can't found this animal");
            }

            var animalDTO = _mapper.Map<List<AnimalDTO>>(animal);

            return animalDTO;

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
            animal.SpeciesId = animalUpdate.SpeciesId;
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
                SpeciesId = animalDTO.SpeciesId,
                DietId = animalDTO.DietId,
                CageId = animalDTO.CageId,
                TrainingPlanId = animalDTO.TrainingPlanId,
            };

            var diet = await _context.Diets.FirstOrDefaultAsync(e => e.Id == animalDTO.DietId);

            if (diet == null)
            {
                animal.Diet = null;
            }

            var cage = await _context.Cages.FirstOrDefaultAsync(e => e.Id == animal.CageId);
            if (cage != null)
            {
                if (cage.AnimalCount == cage.AnimalLimit)
                {
                    return BadRequest("This cage is full");
                }
                else
                {
                    cage.AnimalCount++;

                    _context.Animals.Add(animal);
                    await _context.SaveChangesAsync();
                }
            }
            else
            {
                return BadRequest("Can't found this cage");
            }

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
        public int SpeciesId { get; set; }
        public int? DietId { get; set; }
        public short? CageId { get; set; }
        public short? TrainingPlanId { get; set; }
    }

}
