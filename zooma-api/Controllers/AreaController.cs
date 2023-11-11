using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class areaController : ControllerBase
    {
        private readonly zoomadbContext _context;
        private readonly IMapper _mapper;
        private readonly IAreaRepository _areaRepository;

        public areaController(zoomadbContext context, IMapper mapper, IAreaRepository areaRepository)
        {
            _context = context;
            _mapper = mapper;
            _areaRepository = areaRepository;
        }
        //get all
        [HttpGet()]
        public ActionResult<IEnumerable<Area>> GetAllAreas()
        {

            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }

            var areas = _areaRepository.GetAllAreas();

            return areas;
        }
        /// <summary>
        /// Return area by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //get area by id
        [HttpGet("{id}")]
        public ActionResult<Area> GetAreaById(short id)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }

            var area = _areaRepository.GetAreaById(id);

            if (area == null)
            {
                return NotFound("No area having that ID");
            }
            return area;
        }
        /// <summary>
        /// Return a list of area have that species
        /// </summary>
        /// <param name="speciesId"></param>
        /// <returns></returns>
        //get area by speciesId
        [HttpGet("species/{speciesId}")]
        public async Task<IActionResult> GetAreaBySpeciesId(short speciesId)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }

            var species = await _context.Species.FirstOrDefaultAsync(n => n.Id == speciesId);

            if (species == null)
            {
                return NotFound("Can't found this species");
            }

            var areaId = _areaRepository.GetAreaBySpeciesId(speciesId);


            if (areaId == null)
            {
                return NotFound("No area having that ID");
            }

            return Ok(areaId);
        }


        //update area
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateArea(int id, AreaUpdate areaUpdate)
        {
            var area = await _context.Areas.FirstOrDefaultAsync(e => e.Id == id);
            if (area == null)
            {
                return NotFound("Can't found this area");
            }
            area.Name = areaUpdate.Name;
            area.Description = areaUpdate.Description;

            _context.Entry(area).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(area);
        }
        private bool AreaExists(int id)
        {
            return _context.Areas.Any(e => e.Id == id);
        }
        public class AreaUpdate
        {
            public string? Name { get; set; }
            public string? Description { get; set; }
        }
    }
}