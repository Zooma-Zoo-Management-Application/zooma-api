﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    [Route("api/dietdetails")]
    [ApiController]
    public class DietDetailsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;
        private readonly IDietRepository _dietRepository;
        private readonly IDietDetailsRepository _dietDetailsRepository;

        public DietDetailsController(zoomadbContext context, IMapper mapper, IDietRepository dietRepository, IDietDetailsRepository dietDetailsRepository)
        {
            _context = context;
            _mapper = mapper;
            _dietRepository = dietRepository;
            _dietDetailsRepository = dietDetailsRepository;
        }

        // Hàm lấy tất cả dietDetails ra
        [HttpGet]
        public ActionResult<IEnumerable<DietDetailDTO>> GetDietDetails()
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }

            var dietDetails = _dietDetailsRepository.GetAllDietDetails();

            var dietDetailsDTO = _mapper.Map<List<DietDetailDTO>>(dietDetails);

            foreach (var dietDetailDTO in dietDetailsDTO)
            {
                if (!string.IsNullOrEmpty(dietDetailDTO.FeedingDate))
                {
                    dietDetailDTO.FeedingDateArray = dietDetailDTO.FeedingDate.Split(',');
                }
            }

            return dietDetailsDTO;
        }

        // Hàm lấy Diet Details Bằng Id
        [HttpGet("{id}")]
        public ActionResult<DietDetailDTO> GetDietDetailById(int id)
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }

            var dietDetail = _dietDetailsRepository.GetDietDetailById(id);


            if (dietDetail == null)
            {
                return NotFound("Can't found this diet detail");
            }

            var dietDetailDTO = _mapper.Map<DietDetailDTO>(dietDetail);

            if (!string.IsNullOrEmpty(dietDetailDTO.FeedingDate))
            {
                dietDetailDTO.FeedingDateArray = dietDetailDTO.FeedingDate.Split(',');
            }

            return dietDetailDTO;
        }

        // Hàm lấy Diet Details Bằng DietId
        /// <summary>
        /// Return a list of diet details by dietId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("diet-details/{id}")]
        public ActionResult<IEnumerable<DietDetailDTO>> GetDietDetailByDietId(int id)
        {
            if (_context.DietDetails == null)
            {
                return NotFound();
            }

            var dietDetail = _dietDetailsRepository.GetDietDetailByDietId(id);


            if (dietDetail == null)
            {
                return NotFound("Can't found this diet detail");
            }

            if(dietDetail.Count == 0)
            {
                return NotFound("This diet is empty");
            }

            var dietDetailsDTO = _mapper.Map<List<DietDetailDTO>>(dietDetail);

            foreach (var dietDetailDTO in dietDetailsDTO)
            {
                if (!string.IsNullOrEmpty(dietDetailDTO.FeedingDate))
                {
                    dietDetailDTO.FeedingDateArray = dietDetailDTO.FeedingDate.Split(',');
                }
            }

            return dietDetailsDTO;
        }


        // Hàm cập nhật diet detail
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDietDetails(int id, DietDetailUpdate dietDetailUpdate)
        {
            var dietDetail = await _context.DietDetails.FirstOrDefaultAsync(e => e.Id == id);

            if(dietDetail == null)
            {
                return NotFound("Can't found this diet detail");
            }

            dietDetail.Name = dietDetailUpdate.Name;
            dietDetail.Description = dietDetailUpdate.Description;
            dietDetail.UpdateAt = DateTime.Now;
            dietDetail.ScheduleAt = dietDetailUpdate.ScheduleAt;
            dietDetail.EndAt = dietDetailUpdate.EndAt;
            
            dietDetail.FeedingDate = string.Join(",", dietDetailUpdate.FeedingDate);

            dietDetail.FeedingTime = dietDetailUpdate.ScheduleAt.Value.TimeOfDay;
            dietDetail.Quantity = dietDetailUpdate.Quantity;
            dietDetail.Status = dietDetailUpdate.Status;
            dietDetail.FoodId = dietDetailUpdate.FoodId;
            dietDetail.DietId = dietDetailUpdate.DietId;

            _context.Entry(dietDetail).State = EntityState.Modified;

            TimeSpan time = (TimeSpan)(dietDetail.EndAt - dietDetail.ScheduleAt);
            if(time < TimeSpan.Zero)
            {
                return BadRequest("The end day can't be sooner than the schedule at");
            }
            else
            {
                try
                {
                    await _context.SaveChangesAsync();
                    var diet = _context.Diets.FirstOrDefault(e => e.Id == dietDetail.DietId);
                    if (diet != null)
                    {
                        diet.TotalEnergyValue = _dietRepository.CountEnergyOfDiet(dietDetail.DietId);
                        _context.Entry(diet).State = EntityState.Modified;
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!DietDetailExists(id))
                    {
                        return NotFound("Can't found this diet details");
                    }
                    else
                    {
                        throw new Exception(ex.Message);
                    }
                }

                return Ok("Update successfully!");
            }
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

            TimeSpan? feedingTime = dietDetailCreate.ScheduleAt.Value.TimeOfDay;

            if (feedingTime < TimeSpan.Zero)
            {
                feedingTime = TimeSpan.Zero; 
            }

            DietDetail dietDetail = new DietDetail
            {
                Name = dietDetailCreate.Name,
                Description = dietDetailCreate.Description,
                CreateAt = DateTime.Now,
                UpdateAt = DateTime.Now,
                ScheduleAt = dietDetailCreate.ScheduleAt,
                EndAt = dietDetailCreate.EndAt,
                FeedingTime = feedingTime,
                FeedingDate = string.Join(",", dietDetailCreate.FeedingDate),
                Quantity = dietDetailCreate.Quantity,
                Status = dietDetailCreate.Status,
                FoodId = dietDetailCreate.FoodId,
                DietId = dietDetailCreate.DietId,
            };

            TimeSpan time = (TimeSpan)(dietDetail.EndAt - dietDetail.CreateAt);

            TimeSpan time1 = (TimeSpan)(dietDetail.EndAt - dietDetail.ScheduleAt);

            if (time < TimeSpan.Zero)
            {
                return BadRequest("The end day can't be sooner than the create day");
            } else if(time1 < TimeSpan.Zero)
            {
                return BadRequest("The end day can't be sooner than the schedule day");
            }
            else
            {
                _context.DietDetails.Add(dietDetail);

                await _context.SaveChangesAsync();
                var diet = _context.Diets.FirstOrDefault(e => e.Id == dietDetail.DietId);
                if (diet != null)
                {
                    diet.TotalEnergyValue = _dietRepository.CountEnergyOfDiet(dietDetail.DietId);
                    _context.Entry(diet).State = EntityState.Modified;
                    await _context.SaveChangesAsync();
                }

                return Ok(new { dietDetailDTO = _mapper.Map<DietDetailDTO>(dietDetail), message = "Create successfully!" });
            }
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
        public DateTime UpdateAt { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int[]? FeedingDate { get; set; }
        public double? Quantity { get; set; }
        public bool Status { get; set; }
        public int DietId { get; set; }
        public int FoodId { get; set; }
    }

    public class DietDetailUpdate
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? EndAt { get; set; }
        public int[]? FeedingDate { get; set; }
        public double? Quantity { get; set; }
        public bool Status { get; set; }
        public int DietId { get; set; }
        public int FoodId { get; set; }
    }
}
