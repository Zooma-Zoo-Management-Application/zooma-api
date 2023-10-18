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
    public class AnimalTrainersController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public AnimalTrainersController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm lấy tất cả zootrainer với những animal phụ trách
        [HttpGet("/get-all-animals-with-trainers")]
        public async Task<ActionResult<IEnumerable<AnimalUserDTO>>> GetAnimalUsers()
        {
            if (_context.AnimalUsers == null)
            {
                return NotFound();
            }

            var zooTrainer = await _context.AnimalUsers.
                                                       Include(a => a.Animal).
                                                       Include(b => b.User).
                                                       ToListAsync();

            var zooTrainerDTO = _mapper.Map<List<AnimalUserDTO>>(zooTrainer);

            return zooTrainerDTO;
        }

        // Hàm lấy tất cả animal do 1 user phụ trách
        [HttpGet("/get-animal-by-zootrainerid/{zooTrainerId}")]
        public async Task<ActionResult<IEnumerable<AnimalUserDTO>>> GetZooTrainerWithAnimalByUserId(int zooTrainerId)
        {
              if (_context.AnimalUsers == null)
              {
                  return NotFound();
              }

              var zooTrainer = _context.Users.FirstOrDefault(a => a.Id == zooTrainerId);

              if(zooTrainer == null)
              {
                  return NotFound("Khong tim thay user nay");
              }

              if (zooTrainer.RoleId != 2)
              {
                 return BadRequest("Nguoi nay khong phai zootrainer");
              }

              var animalOfZooTrainer = await _context.AnimalUsers.Where(a => a.UserId == zooTrainerId).
                                                                                                       Include(a => a.Animal).
                                                                                                       Include(b => b.User).   
                                                                                                       ToListAsync();

             if (animalOfZooTrainer.Count == 0)
             {
                return NotFound("Khong tim thay animal cua nhung zootrainer nay");
             }

             var animalOfZooTrainerDTO = _mapper.Map<List<AnimalUserDTO>>(animalOfZooTrainer);

             return animalOfZooTrainerDTO;
        }

        // Hàm lấy tất cả user phụ trách 1 animal
        [HttpGet("/get-zootrainer-by-animalId/{animalId}")]
        public async Task<ActionResult<IEnumerable<AnimalUserDTO>>> GetAllZooTrainerByAnimalId(int animalId)
        {
            if (_context.AnimalUsers == null)
            {
                return NotFound();
            }

            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);

            if(animal == null)
            {
                return NotFound("Khong tim thay animal nay");
            }

            var zooTrainersOfAnimals = await _context.AnimalUsers.Where(a => a.AnimalId == animalId).
                                                                                                     Include(a => a.Animal).
                                                                                                     Include(b => b.User).
                                                                                                     ToListAsync();

            if (zooTrainersOfAnimals.Count == 0)
            {
                return NotFound("Khong tim thay zootrainer cua animal nay");
            }

            var zooTrainersOfAnimalsDTO = _mapper.Map<List<AnimalUserDTO>>(zooTrainersOfAnimals);

            return zooTrainersOfAnimalsDTO;
        }

        // Hàm thay đổi trainer cho 1 con animal
  /*      [HttpPut("/update-animal-trainers-by-animalId/{animalId}")]
        public async Task<IActionResult> UpdateAnimalAssignToTrainerByAnimalId(int animalId, AnimalWithZooTrainerUpdate animalUser)
        {
            var animal = _context.Animals.FirstOrDefault(a => a.Id == animalId);

            if(animal == null)
            {
                return NotFound("Khong tim thay animal nay");
            }

            var animalWithTrainer = _context.AnimalUsers.FirstOrDefault(a => a.AnimalId == animalId);

            animalWithTrainer.UserId = animalUser.UserId;
            animalWithTrainer.MainTrainer = animalUser.MainTrainer;

            _context.Entry(animalWithTrainer).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalUserExists(animalId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update succesfully");
        } */

        // Hàm thay đổi animal cho zootrainers
        [HttpPut("/update-animal-trainers-by-trainerId/{zooTrainerId}")]
        public async Task<IActionResult> UpdateAnimalAssignToTrainerByTrainerId(int zooTrainerId, ZooTrainerWithAnimalUpdate animalUser)
        {

            var zootrainer = _context.Users.FirstOrDefault(a => a.Id == zooTrainerId);

            if (zootrainer == null)
            {
                return NotFound("Khong tim thay user nay");
            }

            if(zootrainer.RoleId != 2)
            {
                return BadRequest("Nguoi nay khong phai la zoo trainer");
            }

            var trainerWithAnimal = _context.AnimalUsers.FirstOrDefault(a => a.UserId == zooTrainerId);

            trainerWithAnimal.AnimalId = animalUser.AnimalId;
            trainerWithAnimal.MainTrainer = animalUser.MainTrainer;

            _context.Entry(trainerWithAnimal).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalUserExists(zooTrainerId))
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

        // Hàm assign animal to zootrainer
        [HttpPost("assign-animal-to-zootrainer")]
        public async Task<ActionResult<AnimalUserDTO>> AssignAnimalToZooTrainer(AnimalWithZooTrainer animalUser)
        {   
            if (_context.AnimalUsers == null)
            {
                return Problem("Entity set 'zoomadbContext.AnimalUsers'  is null.");
            }

            AnimalUser newAnimalWithUser = new AnimalUser
            {
                AnimalId = animalUser.AnimalId,
                UserId = animalUser.UserId,
                MainTrainer = animalUser.MainTrainer
            };

            var animal = _context.Animals.FirstOrDefault(x => x.Id == newAnimalWithUser.AnimalId);

            if (animal == null)
            {
                return NotFound("Khong tim thay con animal nay");
            }

            var zootrainer = _context.Users.FirstOrDefault(x => x.Id == newAnimalWithUser.UserId);

            if(zootrainer == null)
            {
                return NotFound("Khong tim thay zootrainer nay");
            }

            if(zootrainer.RoleId != 2)
            {
                return BadRequest("Nguoi nay khong phai zootrainer");
            }

            _context.AnimalUsers.Add(newAnimalWithUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AnimalUserExists(newAnimalWithUser.AnimalId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { newAnimalWithUserDTO = _mapper.Map<AnimalUserDTO>(newAnimalWithUser), message = "Assign animal successfully" });
        }

        // DELETE: api/ZooTrainers/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAnimalUser(int id)
        {
            if (_context.AnimalUsers == null)
            {
                return NotFound();
            }
            var animalUser = await _context.AnimalUsers.FindAsync(id);
            if (animalUser == null)
            {
                return NotFound();
            }

            _context.AnimalUsers.Remove(animalUser);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AnimalUserExists(int id)
        {
            return (_context.AnimalUsers?.Any(e => e.AnimalId == id)).GetValueOrDefault();
        }
    }

    public class AnimalWithZooTrainer
    {
        public int AnimalId { get; set; }
        public short UserId { get; set; }
        public bool MainTrainer { get; set; }
    }

    public class AnimalWithZooTrainerUpdate
    {
        public short UserId { get; set; }
        public bool MainTrainer { get; set; }
    }

    public class ZooTrainerWithAnimalUpdate
    {
        public int AnimalId { get; set; }
        public bool MainTrainer { get; set; }
    }
}
