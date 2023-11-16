using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    [Route("api/skills")]
    [ApiController]
    public class SkillsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;
        private readonly ISkillRepository _skillRepository;

        public SkillsController(zoomadbContext context, IMapper mapper, ISkillRepository skillRepository)
        {
            _context = context;
            _mapper = mapper;
            _skillRepository = skillRepository;
        }

        // Hàm lấy tất cả skill trong database ra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<SkillDTO>>> GetAllSkills()
        {
            if (_context.Skills == null)
            {
                return NotFound();
            }
            
            var skills = _skillRepository.GetAllSkills();
            
            var skilsDTO = _mapper.Map<List<SkillDTO>>(skills);

            return skilsDTO;
        }

        // Hàm lấy skill dựa trên skillId
        [HttpGet("{id}")]
        public async Task<ActionResult<SkillDTO>> GetSkill(short id)
        {
            if (_context.Skills == null)
            {
                return NotFound();
            }

            var skill = _skillRepository.GetSkillById(id);

            if (skill == null)
            {
                return NotFound("Khong tim thay skill nay");
            }

            var skillDTO = _mapper.Map<SkillDTO>(skill);

            return skillDTO;
        }

        // Hàm cập nhật skill
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateSKill(short id, SkillUpdate skillUpdate)
        {
            var skill = await _context.Skills.FirstOrDefaultAsync(a => a.Id == id);

            skill.Name = skillUpdate.Name;
            skill.Description = skillUpdate.Description;
            skill.Status = skillUpdate.Status;

            _context.Entry(skill).State = EntityState.Modified;

            var skillExists = _context.Skills.Count(e => e.Name == skillUpdate.Name);
            
            if(skillExists > 1)
            {
                return BadRequest("There's already has this skill before");
            }
            else
            {
                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SkillExists(id))
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
        }

        // Hàm tạo 1 skill mới
        [HttpPost]
        public async Task<ActionResult<Skill>> CreateSkill(SkillCreate skillCreate)
        {
            if (_context.Skills == null)
            {
                return Problem("Entity set 'zoomadbContext.Skills'  is null.");
            }
            Skill skill = new Skill
            {
                Name = skillCreate.Name,
                Description = skillCreate.Description,
                Status = true
            };

            _context.Skills.Add(skill);

            var skillExist = _context.Skills.FirstOrDefault(e => e.Name == skillCreate.Name);
            if (skillExist != null)
            {
                return BadRequest("There's already a skill before");
            }
            else
            {
                await _context.SaveChangesAsync();

                return Ok(new { skillDTO = _mapper.Map<SkillDTO>(skill), message = "Create skill successfully" });
            }
        }

        // Hàm xóa skill
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteSkill(short id)
        {
            if (_context.Skills == null)
            {
                return NotFound();
            }
            var skill = await _context.Skills.FindAsync(id);
            if (skill == null)
            {
                return NotFound();
            }

            _context.Skills.Remove(skill);
            await _context.SaveChangesAsync();

            return Ok("Delete successfully");
        }

        private bool SkillExists(short id)
        {
            return (_context.Skills?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class SkillCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
    public class SkillUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
    }
}
