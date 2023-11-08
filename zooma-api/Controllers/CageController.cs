using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

[Route("api/[controller]")]
[ApiController]
public class cageController : ControllerBase
{
    public zoomadbContext _context = new zoomadbContext();
    private readonly IMapper _mapper;

    public cageController(zoomadbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    //Hàm lấy tất cả cage
    [HttpGet()]
    public async Task<ActionResult<IEnumerable<CagesDTO>>> GetAllCages()
    {
        var cages = await _context.Cages.ToListAsync();
        if (cages == null)
        {
            return NotFound();
        }
        var cagesDTO = _context.Cages.Select(c => new CagesDTO
        {
            Id = c.Id,
            Name = c.Name,
            AnimalLimit = c.AnimalLimit,
            AnimalCount = c.AnimalCount,
            Description = c.Description,
            Status = c.Status,
            AreaId = c.AreaId
        });
        return Ok(cagesDTO);
    }
    //Get cage by ID 
    [HttpGet("{id}")]
    public async Task<ActionResult<CagesDTO>> GetCageById(short id)
    {
        if (_context.Cages == null)
        {
            return NotFound();
        }
        var cage = await _context.Cages.FindAsync(id);
        if (cage == null)
        {
            return NotFound();
        }
        return Ok(cage);
    }

    //Hàm tạo cage mới
    [HttpPost()]
    public async Task<ActionResult<CagesDTO>> CreateCage(CageUpdate cageCreate)
    {
        if (_context.Cages == null)
        {
            return NotFound();
        }
        Cage cage = new Cage
        {
            Name = cageCreate.Name,
            AnimalLimit = (byte)cageCreate.AnimalLimit,
            AnimalCount = 0,
            Description = cageCreate.Description,
            Status = true,
            AreaId = (short)cageCreate.AreaId
        };

        var cagesExist = _context.Cages.FirstOrDefault(e => e.Name == cageCreate.Name);

        if (cagesExist != null)
        {
            return BadRequest("This cages is existed before!");
        }
        else
        {
            _context.Cages.Add(cage);
            await _context.SaveChangesAsync();
            return Ok(new { cageDTO = _mapper.Map<CagesDTO>(cage), message = "Created successfully" });
        }
    }

    // hàm lấy Cage dựa trên AreaId kèm theo Animal
    /// <summary>
    /// Return a list of cage base on areaId
    /// </summary>
    /// <param name="areaId"></param>
    /// <returns></returns>
    [HttpGet("area/{areaId}")]
    public async Task<ActionResult<IEnumerable<CagesDTO>>> GetCagesByAreaId(int areaId)
    {
        if (_context.Cages == null)
        {
            return NotFound();
        }

        var area = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaId);

        if (area == null)
        {
            return NotFound("Khong tim thay area nay");
        }

        var cages = _context.Cages
            .Where(a => a.AreaId == area.Id)
            .Where(a => a.Status == true)
            .Include(b => b.Animal)
            .ToList();

        if (cages.Count == 0 || cages == null)
        {
            return NotFound("Khong tim thay cage trong area nay");
        }

        var cagesDTO = _mapper.Map<List<CagesDTO>>(cages);

        return cagesDTO;
    }



    //  Animal animalsByName = await _context.Animals.Where(a => a.Name.Contains(name));



    /// <summary>
    /// Cage-removal
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("removal/{id}")]
    public async Task<IActionResult> DeleteCage(int id)
    {
        if (_context.Cages == null)
        {
            return Problem("Entity set 'ZoomaContext.Cages'  is null.");
        }

        var cage = await _context.Cages.FirstOrDefaultAsync(e => e.Id == id);



        if (cage != null)
        {
            if (cage.Status == false)
            {
                return BadRequest("This cage is invalid");
            }

            var animal = cage.AnimalCount;
            if (animal > 0)
            {
                return BadRequest("Cant' remove this cage because there is still animal in it");
            }

            else
            {
                cage.Status = false;
                await _context.SaveChangesAsync();
            }
        }

        else
        {
            return NotFound("Can't found this cage");
        }

        return Ok("Delete successfully!");
    }

    //Hàm update cage
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCage(int id, CageUpdate cage)
    {
        var cageUpdate = await _context.Cages.SingleOrDefaultAsync(c => c.Id == id);
        if (cageUpdate == null)
        {
            return BadRequest();
        }
        else if (cageUpdate.Status == false)
        {
            return BadRequest("This cage is not available to update");
        }
        else
        {
            cageUpdate.Name = cageUpdate.Name ?? cage.Name;
            cageUpdate.AnimalLimit = (byte)cage.AnimalLimit;
            cageUpdate.Description = cage.Description;
            cageUpdate.AreaId = (short)cage.AreaId;
            _context.Entry(cageUpdate).State = EntityState.Modified;
        }
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!CageExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }
        return Ok(new { cage = _mapper.Map<CagesDTO>(cageUpdate), message = "Cage updated successfully" });
    }

    /// <summary>
    /// Status
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpPut("status/{id}")]
    public async Task<IActionResult> UpdateCageStatus(int id)
    {
        if (_context.Cages == null)
        {
            return Problem("Entity set 'ZoomaContext.Cages' is null.");
        }

        var cage = await _context.Cages.FirstOrDefaultAsync(e => e.Id == id);

        if (cage != null)
        {
            if (cage.Status == true)
            {
                return BadRequest("This cage is invalid");
            }

            cage.Status = true;
            await _context.SaveChangesAsync();
        }

        else
        {
            return NotFound("Can't found this cage");
        }

        return Ok("Update successfully!");
    }

    private bool CageExists(int id)
    {
        return _context.Cages.Any(e => e.Id == id);
    }

    public class CageUpdate
    {

        public string Name { get; set; }
        public short AnimalLimit { get; set; }
        public string Description { get; set; }
        public int AreaId { get; set; }
    }
}
