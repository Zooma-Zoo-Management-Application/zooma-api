using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DietsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;

        public DietsController(zoomadbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //Hàm lấy tất cả các diet
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<DietDTO>>> GetDiets()
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }

            var diet = await _context.Diets.Include(a => a.Animals).ToListAsync();
            
            if (diet == null)
            {
                return NotFound("No diet is available");
            }
            
            var dietDTO = _mapper.Map<List<DietDTO>>(diet);

            return dietDTO;
        }

        // Hàm lấy Diet dựa trên Id
        [HttpGet("{id}")]
        public async Task<ActionResult<DietDTO>> GetDietById(int id)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }

            var diet = await _context.Diets.Include(a => a.Animals).FirstOrDefaultAsync(e => e.Id == id);

            if (diet == null)
            {
                return NotFound("There is no diet match your found");
            }

            var dietDTO = _mapper.Map<DietDTO>(diet);

            return dietDTO;
        }
        // GET: api/Diets/5
        [HttpGet("GetDietByName/{name}")]
        public async Task<ActionResult<Diet>> GetDietByName(string name)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            var diet = await _context.Diets.SingleOrDefaultAsync(d => d.Name == name);

            if (diet == null)
            {
                return NotFound();
            }

            return diet;
        }

        //Hàn update diet
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDiet(int id, DietUpdate dietUpdate)
        {
            var diet = await _context.Diets.FirstOrDefaultAsync(e => e.Id == id);

            if (diet == null)
            {
                return NotFound("Can't found this diet");
            }
            else
            {
                var dietDetailsId = _context.DietDetails.Where(e => e.DietId == diet.Id).Include(e => e.Food).ToList();
                
                double totalEnergy = 0;

                foreach (var item in dietDetailsId)
                {
                    if (item != null && item.Food != null)
                    {
                        totalEnergy += item.Food.EnergyValue * (double) item.Quantity;
                    }
                }

                DateTime startDay = diet.ScheduleAt;
                DateTime endDay = diet.EndAt;
                TimeSpan allDay = endDay - startDay;
                if (allDay < TimeSpan.Zero)
                {
                    allDay = TimeSpan.FromDays(1);
                }

                double totalDay = (double) allDay.TotalDays; 

                totalEnergy = totalEnergy / totalDay; 

                diet.Name = dietUpdate.Name ?? diet.Name;
                diet.Description = dietUpdate.Description;
                //        dietUpdate.CreateAt = dietUpdate.CreateAt;
                diet.UpdateAt = dietUpdate.UpdateAt;
                diet.ScheduleAt = dietUpdate.ScheduleAt;
                diet.Status = dietUpdate.Status;
                diet.Goal = dietUpdate.Goal;
                diet.EndAt = dietUpdate.EndAt;
                diet.TotalEnergyValue = totalEnergy;

                try
                {
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietExists(id))
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

        // Hàm tạo diet mới
        [HttpPost()]
        public async Task<ActionResult<Diet>> CreateDiet(DietCreate dietCreate)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }

            Diet diet = new Diet
            {
                Name = dietCreate.Name,
                Description = dietCreate.Description,
                CreateAt = dietCreate.CreateAt,
                UpdateAt = dietCreate.UpdateAt,
                ScheduleAt = dietCreate.ScheduleAt,
                Status = false,
                Goal = dietCreate.Goal,
                EndAt = dietCreate.EndAt,
                TotalEnergyValue = 0
            };

            var dietExists = _context.Diets.FirstOrDefault(e => e.Name == diet.Name);

            if(dietExists != null)
            {
                return BadRequest("This diets is existed");
            }

            _context.Diets.Add(diet);
            await _context.SaveChangesAsync();

            return Ok(new {dietDTO = _mapper.Map<DietDTO>(diet), message = "Create successfully!"});

        }

        private bool DietExists(int id)
        {
            return (_context.Diets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        

        public class DietUpdate
        {
            public string Name { get; set; } = null!;
            public string? Description { get; set; }
            public DateTime UpdateAt { get; set; }
            public DateTime ScheduleAt { get; set; }
            public bool Status { get; set; }
            public string Goal { get; set; } = null!;
            public DateTime EndAt { get; set; }
        }
        public class DietCreate
        {
            public string Name { get; set; }
            public string? Description { get; set; }
            public DateTime CreateAt { get; set; }
            public DateTime UpdateAt { get; set; }
            public DateTime ScheduleAt { get; set; }
            public string Goal { get; set; }
            public DateTime EndAt { get; set; }
        }
    }
}