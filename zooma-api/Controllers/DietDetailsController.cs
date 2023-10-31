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
    public class DietDetailsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public DietDetailsController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Hàm lấy tất cả dietDetails ra
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DietDetailDTO>>> GetDietDetails()
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }

            var dietDetails = await _context.DietDetails.
                                                        Include(e => e.Diet).
                                                        Include(e => e.Food).
                                                        ToListAsync();

            var dietDetailsDTO = _mapper.Map<List<DietDetailDTO>>(dietDetails);

            return dietDetailsDTO;
        }

        // Hàm lấy Diet Details Bằng Id
        [HttpGet("{id}")]
        public async Task<ActionResult<DietDetailDTO>> GetDietDetailById(int id)
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }

            var dietDetail = await _context.DietDetails.
                                                        Include(e => e.Diet).
                                                        Include(e => e.Food).
                                                        FirstOrDefaultAsync(e => e.Id == id);


            if (dietDetail == null)
            {
                return NotFound("Can't found this diet detail");
            }

            var dietDetailDTO = _mapper.Map<DietDetailDTO>(dietDetail); 

            return dietDetailDTO;
        }

        // Hàm lấy Diet Details Bằng DietId
        [HttpGet("/get-diet-details-by-diet-Id/{id}")]
        public async Task<ActionResult<IEnumerable<DietDetailDTO>>> GetDietDetailByDietId(int id)
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }

            var dietDetail = await _context.DietDetails.
                                                        Include(e => e.Diet).
                                                        Include(e => e.Food).
                                                        Where(e => e.DietId == id).
                                                        ToListAsync();


            if (dietDetail == null)
            {
                return NotFound("Can't found this diet detail");
            }

            if(dietDetail.Count == 0)
            {
                return NotFound("This diet is empty");
            }

            var dietDetailDTO = _mapper.Map<List<DietDetailDTO>>(dietDetail);

            return dietDetailDTO;
        }


        // Hàm cập nhật diet detail
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDietDetails(int id, DietDetailUpdate dietDetailUpdate)
        {
            var  dietDetail = await _context.DietDetails.FirstOrDefaultAsync(e => e.Id == id);

            if(dietDetail == null)
            {
                return NotFound("Can't found this diet detail");
            }

            dietDetail.Name = dietDetailUpdate.Name;
            dietDetail.Description = dietDetailUpdate.Description;
            dietDetail.UpdateAt = DateTime.Now;
            dietDetail.EndAt = dietDetailUpdate.EndAt;
            dietDetail.FeedingInterval = dietDetailUpdate.FeedingInterval;
            dietDetail.Status = dietDetailUpdate.Status;
            dietDetail.FoodId = dietDetailUpdate.FoodId;
            dietDetail.DietId = dietDetailUpdate.DietId;

            _context.Entry(dietDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DietDetailExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Update successfully!");
        }

        // POST: api/DietDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DietDetail>> CreateDietDetail(DietDetailCreate dietDetailCreate)
        {
            if (_context.DietDetails == null)
            {
              return Problem("Entity set 'zoomadbContext.DietDetails'  is null.");
            }

            DietDetail dietDetail = new DietDetail
            {
                Name = dietDetailCreate.Name,
                Description = dietDetailCreate.Description,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                ScheduleAt = dietDetailCreate.ScheduleAt,
                EndAt = dietDetailCreate.EndAt,
                FeedingInterval = dietDetailCreate.FeedingInterval,
                Status = dietDetailCreate.Status,
                FoodId = dietDetailCreate.FoodId,
                DietId = dietDetailCreate.DietId,
            };

            var dietDetailExists = _context.DietDetails.FirstOrDefault(e => e.Name == dietDetailCreate.Name);

            if (dietDetailExists != null)
            {
                return BadRequest("This diet exists before!");
            }

            _context.DietDetails.Add(dietDetail);
            await _context.SaveChangesAsync();

            return Ok(new { dietDetailDTO = _mapper.Map<DietDetailDTO>(dietDetail), message = "Create successfully!"});
        }

        // DELETE: api/DietDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDietDetail(int id)
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }
            var dietDetail = await _context.DietDetails.FindAsync(id);
            if (dietDetail == null)
            {
                return NotFound();
            }

            _context.DietDetails.Remove(dietDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DietDetailExists(int id)
        {
            return (_context.DietDetails?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

    public class DietDetailCreate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? EndAt { get; set; }
        public short FeedingInterval { get; set; }
        public bool Status { get; set; }
        public int DietId { get; set; }
        public int FoodId { get; set; }
    }

    public class DietDetailUpdate
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? EndAt { get; set; }
        public short FeedingInterval { get; set; }
        public bool Status { get; set; }
        public int DietId { get; set; }
        public int FoodId { get; set; }
    }
}
