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
    public class TrainerExpsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public TrainerExpsController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm lấy tất cả các kinh nghiệm của trainer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TrainerExpDTO>>> GetAllTrainerExps()
        {
            if (_context.TrainerExps == null)
            {
                return NotFound();
            }

            var trainerExp = await _context.TrainerExps.
                                                        Include(a => a.Skill).
                                                        Include(a => a.User).
                                                        ToListAsync();
            
            var trainerExpDTO = _mapper.Map<List<TrainerExpDTO>>(trainerExp);

            return Ok(trainerExpDTO);
        }

        //Hàm lấy kinh nghiệm bởi Id
        [HttpGet("{Id}")]
        public async Task<ActionResult<TrainerExpDTO>> GetTrainerExp(int Id)
        {
            if (_context.TrainerExps == null)
            {
                return NotFound();
            }

            var trainerExp = await _context.TrainerExps.
                                            Include(a => a.Skill).
                                            Include(a => a.User).
                                            FirstOrDefaultAsync(a => a.Id == Id);

            if (trainerExp == null)
            {
                return NotFound("Can't found");
            }

            var trainerExpDTO = _mapper.Map<TrainerExpDTO>(trainerExp);

            return trainerExpDTO;
        }

        // Hàm lấy kinh nghiệm của trainer bởi trainerId
        /// <summary>
        /// Get Trainer Exp by Trainer Id
        /// </summary>
        /// <param name="trainerId"></param>
        /// <returns></returns>
        [HttpGet("trainer/{trainerId}")]
        public async Task<ActionResult<IEnumerable<TrainerExpDTO>>> GetTrainerExpByTrainerId(int trainerId)
        {
            if (_context.TrainerExps == null)
            {
                return NotFound();
            }

            var trainer = _context.Users.FirstOrDefault(a => a.Id == trainerId);

            if (trainer == null)
            {
                return NotFound("Can't found this trainer");
            }

            if(trainer.RoleId != 2)
            {
                return BadRequest("This user is not a trainer");
            }

            var trainerExp = await _context.TrainerExps.
                                                        Where(a => a.UserId == trainerId).
                                                        Include(a => a.Skill).
                                                        Include(a => a.User).
                                                        ToListAsync();

            if (trainerExp == null)
            {
                return NotFound("Skill issue!!!");
            }

            var trainerExpDTO = _mapper.Map<List<TrainerExpDTO>>(trainerExp);

            return trainerExpDTO;
        }


        // Hàm update skill details của trainer
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTrainerExp(int id, TrainerExpUpdate trainerExpUpdate)
        {
            var trainerExp = _context.TrainerExps.FirstOrDefault(a => a.Id == id);

            if (trainerExp == null)
            {
                return NotFound("NOT FOUND!!!");
            }

            trainerExp.YearOfExperience = trainerExpUpdate.YearOfExperience;
            trainerExp.Status = trainerExpUpdate.Status;
            trainerExp.Description = trainerExpUpdate.Description;

            _context.Entry(trainerExp).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TrainerExpExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update successfully");
        }

        // Hàm assign skill mới cho trainer
        [HttpPost]
        public async Task<ActionResult<TrainerExp>> AssignSkillForTrainer(TrainerExpCreate trainerExpCreate)
        {
            if (_context.TrainerExps == null)
            {
                return Problem("Entity set 'zoomadbContext.TrainerExps'  is null.");
            }

            TrainerExp trainerExp = new TrainerExp
            {
                YearOfExperience = trainerExpCreate.YearOfExperience,
                Description = trainerExpCreate.Description,
                UserId = trainerExpCreate.UserId,
                SkillId = trainerExpCreate.SkillId,
            };

            var skill = _context.Skills.FirstOrDefault(x => x.Id == trainerExp.SkillId);

            if(skill == null)
            {
                return NotFound("Can't found this skill");
            }

            var trainer = _context.Users.FirstOrDefault(x => x.Id == trainerExp.UserId); 
            
            if(trainer == null)
            {
                return NotFound("Can't found this trainer");
            }

            if (trainer.RoleId != 2)
            {
                return BadRequest("This user is not a trainer");
            }

            var existingTrainerExp = _context.TrainerExps.FirstOrDefault(te => te.UserId == trainerExp.UserId && te.SkillId == trainerExp.SkillId);

            if (existingTrainerExp != null)
            {
                return Conflict("This skill is already assigned to the trainer");
            }

            _context.TrainerExps.Add(trainerExp);
            await _context.SaveChangesAsync();

            return Ok(new { trainerExpDTO = _mapper.Map<TrainerExpDTO>(trainerExp), message = "Create successfully" });
        }

        // Hàm xóa skill của trainer
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTrainerExp(int id)
        {
            if (_context.TrainerExps == null)
            {
                return NotFound();
            }
            var trainerExp = await _context.TrainerExps.FindAsync(id);
            if (trainerExp == null)
            {
                return NotFound("Can't find this skill of the trainer");
            }

            _context.TrainerExps.Remove(trainerExp);
            await _context.SaveChangesAsync();

            return Ok("Delete successfully");
        }

        private bool TrainerExpExists(int id)
        {
            return (_context.TrainerExps?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class TrainerExpCreate
    {
        public byte YearOfExperience { get; set; }
        public string Description { get; set; }
        public short UserId { get; set; }
        public short SkillId { get; set; }
    }

    public class TrainerExpUpdate
    {
        public byte YearOfExperience { get; set; }
        public byte Status { get; set; }
        public string Description { get; set; }
    }
}
