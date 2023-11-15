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
    [Route("api/animals")]
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
        /// <summary>
        /// Return a list of animal base on cageId
        /// </summary>
        /// <param name="cageId"></param>
        /// <returns></returns>
        [HttpGet("cage/{cageId}")]
        public ActionResult<IEnumerable<AnimalDTO>> GetAnimalsByCageId(int cageId)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var cages = _context.Cages.FirstOrDefault(a => a.Id == cageId);

            if (cages == null)
            {
                return NotFound("Cage Not Found!!");
            }

            var animals = _animalRepository.GetAnimalsByCageId(cageId);

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return animalDTOs;
        }

        // ham lay animal dua tren AreaId
        /// <summary>
        /// Return a list of animal base on area Id
        /// </summary>
        /// <param name="areaId"></param>
        /// <returns></returns>
        [HttpGet("area/{areaId}")]
        public async Task<ActionResult<IEnumerable<AnimalDTO>>> GetAnimalsByAreaId(int areaId)
        {
            if (_context.Animals == null)
            {
                return NotFound();
            }

            var area = _context.Areas.FirstOrDefault(a => a.Id == areaId);

            if (area == null)
            {
                return NotFound("Khong tim thay area nay");
            }

            var animals = _animalRepository.GetAnimalsByAreaId(areaId);

            if (animals == null)
            {
                return NotFound();
            }

            var animalDTOs = _mapper.Map<List<AnimalDTO>>(animals);

            return animalDTOs;
        }

        // ham lay animal dua tren CageId = null
        /// <summary>
        /// Return a list of animal without cage
        /// </summary>
        /// <returns></returns>
        [HttpGet("cageless")]
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

            animal.Name = animalUpdate.Name;
            animal.Height = animalUpdate.Height;
            animal.Weight = animalUpdate.Weight;
            animal.Description = animalUpdate.Description;
            animal.Status = animalUpdate.Status;
            animal.SpeciesId = animalUpdate.SpeciesId;
            animal.DietId = animalUpdate.DietId;
            animal.CageId = animalUpdate.CageId;
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
                return Problem("Entity set 'ZoomaContext.Animals' is null.");
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

     /*       var diet = await _context.Diets.FirstOrDefaultAsync(e => e.Id == animalDTO.DietId);

            if (diet == null)
            {
                animal.DietId = null;
            } */
            
            TimeSpan time = animal.ArrivalDate - animalDTO.DateOfBirth;

            if (time < TimeSpan.Zero)
            {
                return BadRequest("The Date Of Birth can't be sooner than Arrival Date!");
            }
            else
            {
                if (animal.CageId == null)
                {
                    animal.CageId = null;
                    _context.Animals.Add(animal);
                    await _context.SaveChangesAsync();
                }
                else
                {
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
                        return BadRequest("Can't found this cage!");
                    }
                }

                return Ok(new { animalDTO = _mapper.Map<AnimalDTO>(animal), message = "Animal created successfully" });
            }
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
            _animalRepository.UpdateAnimalInCage();

            return Ok("Delete successfully");
        }

        //select a list of animal from the database and unassign cage from them
        /// <summary>
        /// Assign a list of animal from their cage
        /// </summary>
        /// <param name="cageID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("cage/{cageID}")]
        public async Task<IActionResult> AssignAnimal(short cageID, [FromBody] int[] id)
        {
            var cage = await _context.Cages.FirstOrDefaultAsync(e => e.Id == cageID);

            if (cage == null)
            {
                return NotFound("Can't found this cage");
            }

            bool status = false;

            var count = id.Count();

            var animalInCage = _context.Animals.Count(e => e.CageId == cageID);

            if (count + animalInCage > cage.AnimalLimit)
            {
                return BadRequest("Cage is full!");
            }
            else
            {
                foreach (var item in id)
                {
                    var animal = _context.Animals.FirstOrDefault(e => e.Id == item);

                    if (animal == null)
                    {
                        return NotFound("Invalid Id (" + item + ")");
                    }
                    else
                    {
                        status = _animalRepository.AssignAnimalToACage(cageID, item);

                        if (status == false)
                        {
                            break;
                        }
                    }
                }
            }

            if (status == true)
            {
                await _context.SaveChangesAsync();
                return Ok("Assign successfully");
            }
            else
            {
                return BadRequest("Cage is full!");
            }
        }
        /// <summary>
        /// Unassign from their cage
        /// </summary>
        /// <param name="cageID"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("id")]
        public async Task<IActionResult> UnssignAnimal(int[] id)
        {
            bool status = false;

            foreach (var item in id)
            {
                var animal = _context.Animals.FirstOrDefault(e => e.Id == item);

                if (animal == null)
                {
                    return NotFound("Invalid Id (" + item + ")");
                }
                else 
                {
                    status = _animalRepository.UnassignAnimal(item);
                        
                    if (status == false)
                    {
                        break;
                    }
                }
            }

            if (status == true)
            {
                await _context.SaveChangesAsync();
                return Ok("Unassign successfully");
            }
            else
            {
                return BadRequest("Something went wrong");
            }
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
