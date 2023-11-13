using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/diets")]
    [ApiController]
    public class DietsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();
        private readonly IMapper _mapper;
        private readonly IDietRepository _dietRepository;

        public DietsController(zoomadbContext context, IMapper mapper, IDietRepository dietRepository)
        {
            _context = context;
            _mapper = mapper;
            _dietRepository = dietRepository;
        }

        //Hàm lấy tất cả các diet
        [HttpGet()]
        public ActionResult<IEnumerable<DietDTO>> GetDiets()
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }

            var diet = _dietRepository.GetAllDiets();

            if (diet == null)
            {
                return NotFound("No diet is available");
            }

            var dietDTO = _mapper.Map<List<DietDTO>>(diet);

            return dietDTO;
        }

        /// <summary>
        /// Return the diet by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // Hàm lấy Diet dựa trên Id
        [HttpGet("{id}")]
        public ActionResult<DietDTO> GetDietById(int id)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }

            var diet = _dietRepository.GetDietById(id);

            if (diet == null)
            {
                return NotFound("There is no diet match your found");
            }

            var dietDTO = _mapper.Map<DietDTO>(diet);

            return dietDTO;
        }

        /// <summary>
        /// Return the diet by name
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        // GET: api/Diets/5
        [HttpGet("name/{name}")]
        public ActionResult<Diet> GetDietByName(string name)
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }
            var diet = _dietRepository.GetDietByName(name);

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

                double totalEnergy = _dietRepository.CountEnergyOfDiet(id);

                diet.Name = diet.Name;
                diet.Description = dietUpdate.Description;
                //        dietUpdate.CreateAt = dietUpdate.CreateAt;
                diet.UpdateAt = dietUpdate.UpdateAt;
                diet.ScheduleAt = dietUpdate.ScheduleAt;
                diet.Status = dietUpdate.Status;
                diet.Goal = dietUpdate.Goal;
                diet.EndAt = dietUpdate.EndAt;
                diet.TotalEnergyValue = totalEnergy;

                TimeSpan time = diet.EndAt - diet.ScheduleAt;

                if(time < TimeSpan.Zero)
                {
                    return BadRequest("The end day can't be sooner than the schedule day");
                }
                else
                {
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
        }

        //Hàm trả về những inactive diet
        /// <summary>
        /// Return a list of inactive diet
        /// </summary>
        /// <returns></returns>
        [HttpGet("status")]
        public ActionResult<IEnumerable<DietDTO>> GetInActive()
        {
            if (_context.Diets == null)
            {
                return NotFound();
            }

            var diet = _dietRepository.GetInActiveDiet();

            if (diet == null)
            {
                return NotFound("There is no diet match your found");
            }

            var dietDTO = _mapper.Map<List<DietDTO>>(diet);

            return dietDTO;
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

            TimeSpan time = diet.EndAt - diet.ScheduleAt;
            TimeSpan time1 = diet.EndAt - diet.CreateAt;

            if (time < TimeSpan.Zero)
            {
                return BadRequest("The end day can't be sooner the schedule day");
            } else if(time1 < TimeSpan.Zero){
                return BadRequest("The end day can't be sooner the create day");
            }
            else
            {
                var dietExists = _context.Diets.FirstOrDefault(e => e.Name == diet.Name);

                if (dietExists != null)
                {
                    return BadRequest("This diets is existed");
                }

                _context.Diets.Add(diet);
                await _context.SaveChangesAsync();

                return Ok(new { dietDTO = _mapper.Map<DietDTO>(diet), message = "Create successfully!" });
            }
        }

        //Hàm xóa diet
        /// <summary>
        /// Change status of diet
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpPut("status/{id}")]
        public async Task<IActionResult> UpdateStatusOfDiet(int id, bool status)
        {
            var diet = await _context.Diets.Include(e => e.Animals).FirstOrDefaultAsync(e => e.Id == id);

            if (diet == null)
            {
                return NotFound("Can't found this diet");
            }
            else
            {
                if (diet.Status == true)
                {
                    if (diet.Animals.Any())
                    {
                        return BadRequest("This diet can't be deleted");
                    }
                    else
                    {
                        diet.Status = status;

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

                        return Ok("Delete successfully");
                    }
                }
                else
                {
                    diet.Status = status;
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
        }

        private bool DietExists(int id)
        {
            return (_context.Diets?.Any(e => e.Id == id)).GetValueOrDefault();
        }


        public class DietUpdate
        {
            public string? Name { get; set; } 
            public string? Description { get; set; }
            public DateTime UpdateAt { get; set; }
            public DateTime ScheduleAt { get; set; }
            public bool Status { get; set; }
            public string? Goal { get; set; } 
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