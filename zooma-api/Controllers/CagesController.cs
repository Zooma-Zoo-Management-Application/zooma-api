using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;
using zooma_api.DTO;
using System.Xml.Linq;

namespace zooma_api.Controllers
{
    public class CagesController : Controller
    {
        private readonly zoomadbContext _context;

        public CagesController(zoomadbContext context)
        {
            _context = context;
        }
        //Hàm lấy tất cả cage
        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<CagesDTO>>> GetAllCages()
        {
            if (_context.Cages == null)
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
        [HttpPut("AssignAnimal/{id}")]
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

        
        // GET: Cages
        public async Task<IActionResult> Index()
        {
            var zoomaContext = _context.Cages.Include(c => c.Area);
            return View(await zoomaContext.ToListAsync());
        }

        // GET: Cages/Details/5
        public async Task<IActionResult> Details(short? id)
        {
            if (id == null || _context.Cages == null)
            {
                return NotFound();
            }

            var cage = await _context.Cages
                .Include(c => c.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cage == null)
            {
                return NotFound();
            }

            return View(cage);
        }

        // GET: Cages/Create
        public IActionResult Create()
        {
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id");
            return View();
        }

        // POST: Cages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,AnimalLimit,AnimalCount,Description,Status,AreaId")] Cage cage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id", cage.AreaId);
            return View(cage);
        }

        // GET: Cages/Edit/5
        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null || _context.Cages == null)
            {
                return NotFound();
            }

            var cage = await _context.Cages.FindAsync(id);
            if (cage == null)
            {
                return NotFound();
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id", cage.AreaId);
            return View(cage);
        }

        // POST: Cages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Id,Name,AnimalLimit,AnimalCount,Description,Status,AreaId")] Cage cage)
        {
            if (id != cage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CageExists(cage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["AreaId"] = new SelectList(_context.Areas, "Id", "Id", cage.AreaId);
            return View(cage);
        }

        // GET: Cages/Delete/5
        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null || _context.Cages == null)
            {
                return NotFound();
            }

            var cage = await _context.Cages
                .Include(c => c.Area)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cage == null)
            {
                return NotFound();
            }

            return View(cage);
        }

        // POST: Cages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
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
            return RedirectToAction(nameof(Index));
        }

        private bool CageExists(short id)
        {
          return (_context.Cages?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
