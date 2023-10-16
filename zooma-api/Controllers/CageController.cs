using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

[Route("api/[controller]")]
[ApiController]
public class CageController : ControllerBase
{
    private readonly zoomadbContext _context;

    public CageController(zoomadbContext context)
    {
        _context = context;
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
    public async Task<ActionResult<CagesDTO>> CreateCage(CagesDTO createCage)
    {
        if (_context.Cages == null)
        {
            return NotFound();
        }
        var cage = new Cage
        {
            Name = createCage.Name,
            AnimalLimit = createCage.AnimalLimit,
            AnimalCount = createCage.AnimalCount,
            Description = createCage.Description,
            Status = createCage.Status,
            AreaId = createCage.AreaId
        };
        _context.Cages.Add(cage);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetCage", new { id = cage.Id }, cage);
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
}