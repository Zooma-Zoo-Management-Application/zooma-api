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
    public class ZooTrainersController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public ZooTrainersController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm lấy zootrainer với id

        [HttpGet("/get-zootrainer-by-id/{id}")]
        public async Task<ActionResult<UserDTO>> GetZooTrainerById(int id)
        {
            if (_context.Users == null)
            {
                return NotFound();
            }
            var zootrainer = _context.Users.FirstOrDefault(a => a.Id == id);

            if (zootrainer == null)
            {
                return NotFound("Khong tim thay zoo trainer nay");
            }

            if(zootrainer.RoleId != 2)
            {
                return BadRequest("Nguoi nay khong phai zoo trainer");
            }

            var zooTrainerDTO = _mapper.Map<UserDTO>(zootrainer);

            return zooTrainerDTO;
        }

        // Hàm lấy tất cả zootrainer với những animal phụ trách
        [HttpGet]
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

        // PUT: api/ZooTrainers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAnimalUser(int id, AnimalUser animalUser)
        {
            if (id != animalUser.AnimalId)
            {
                return BadRequest();
            }

            _context.Entry(animalUser).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AnimalUserExists(id))
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

        // POST: api/ZooTrainers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AnimalUser>> PostAnimalUser(AnimalUser animalUser)
        {
          if (_context.AnimalUsers == null)
          {
              return Problem("Entity set 'zoomadbContext.AnimalUsers'  is null.");
          }
            _context.AnimalUsers.Add(animalUser);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (AnimalUserExists(animalUser.AnimalId))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetAnimalUser", new { id = animalUser.AnimalId }, animalUser);
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
}
