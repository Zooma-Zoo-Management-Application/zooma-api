using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

[Route("api/[controller]")]
[ApiController]
public class CageController : ControllerBase
{
    public zoomadbContext _context = new zoomadbContext();
    private readonly IMapper _mapper;

    public CageController(zoomadbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    //Hàm lấy tất cả cage
    [HttpGet("GetAllCages")]
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
    [HttpGet("GetCageById/{id}")]
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
    [HttpPost("CreateCage")]
    public async Task<ActionResult<CagesDTO>> CreateCage(CageUpdate cage)
    {
        if (_context.Cages == null)
        {
            return NotFound();
        }
        Cage cageUpdate = new Cage
        {
            Name = cage.Name,
            AnimalLimit = (byte)cage.AnimalLimit,
            AnimalCount = (byte)cage.AnimalCount,
            Description = cage.Description,
            Status = cage.Status,
            AreaId = (short)cage.AreaId
        };
        _context.Cages.Add(cageUpdate);
        await _context.SaveChangesAsync();
        return CreatedAtAction("GetCageById", new { id = cageUpdate.Id }, cageUpdate);

    }
    //select an animal from the database and assign to the cage 
    [HttpPut("AssignAnimal/{id}/{cageID}")]
    public async Task<IActionResult> AssignAnimal(int id, int cageID)
    {
        var animal = await _context.Animals.FindAsync(id);
        var cageDTO = await _context.Cages.FindAsync(cageID);
        if (animal == null || cageDTO == null)
        {
            return NotFound("No animals available");
        }
        if (cageDTO.AnimalCount < cageDTO.AnimalLimit)
        {
            cageDTO.AnimalCount++;
            animal.CageId = (short?)cageID;
            await _context.SaveChangesAsync();
            return Ok("Animal assigned to cage");
        }
        else
        {
            return BadRequest("Cage is full");
        }
    }

    // hàm lấy Cage dựa trên AreaId kèm theo Animal
    [HttpGet("get-cages-by-areaId/{id}")]
    public async Task<ActionResult<IEnumerable<CagesDTO>>> GetCagesByAreaId(int id)
    {
        if (_context.Cages == null)
        {
            return NotFound();
        }

        var area = _context.Areas.FirstOrDefault(a => a.Id == id);

        if (area == null)
        {
            return NotFound("Khong tim thay area nay");
        }

        var cages = _context.Cages
            .Where(a => a.AreaId == area.Id)
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


    //Hàm xóa cage
    [HttpDelete("DeleteCage/{id}")]
    public async Task<IActionResult> DeleteCage(int id)
    {
        if (_context.Cages == null)
        {
            return Problem("Entity set 'ZoomaContext.Cages'  is null.");
        }
        var cage = await _context.Cages.FindAsync(id);
        if (cage != null)
        {
            _context.Cages.Remove(cage);
        }

        await _context.SaveChangesAsync();
        return NoContent();
    }
    //Hàm update cage
    [HttpPut("UpdateCage/{id}")]
    public async Task<IActionResult> UpdateCage(int id, CageUpdate cage)
    {
        var cageUpdate = await _context.Cages.SingleOrDefaultAsync(c => c.Id == id);
        if (cageUpdate == null)
        {
            return BadRequest();
        }
        else
        {
            cageUpdate.Name = cageUpdate.Name ?? cage.Name;
            cageUpdate.AnimalLimit = (byte)cage.AnimalLimit;
            cageUpdate.AnimalCount = (byte)cage.AnimalCount;
            cageUpdate.Description = cageUpdate.Description ?? cage.Description;
            cageUpdate.Status = cage.Status;
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

    private bool CageExists(int id)
    {
        return _context.Cages.Any(e => e.Id == id);
    }

    public class CageUpdate
    {

        public string Name { get; set; }
        public short AnimalLimit { get; set; }
        public short AnimalCount { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }
        public int AreaId { get; set; }
    }
}
