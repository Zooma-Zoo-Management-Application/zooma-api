using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;
using zooma_api.DTO;


namespace zooma_api.Controllers
{
    public class AreaController : Controller
    {
        private readonly zoomadbContext _context;

        public AreaController(zoomadbContext context)
        {
            _context = context;
        }
        // Get all areas in the system
        [HttpGet]
        [Route("api/areas")]
        public async Task<ActionResult<IEnumerable<AreaDTO>>> GetAreas()

        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }

            return await _context.Areas.Select(a => new AreaDTO(a)).ToListAsync();
        }
        // Get detailed information of an area
        [HttpGet]
        [Route("api/areas/{id}")]
        public async Task<ActionResult<AreaDTO>> GetArea(short id)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            var area = await _context.Areas.FindAsync(id);

            if (area == null)
            {
                return NotFound("Area not found");
            }

            return new AreaDTO(area);
        }
        // Create a new area
        [HttpPost]
        [Route("api/areas")]
        public async Task<ActionResult<AreaDTO>> PostArea(Area area)
        {

            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArea", new { id = area.Id }, new AreaDTO(area));
        }
        // Update an area
        [HttpPut]
        [Route("api/areas/{id}")]
        public async Task<IActionResult> UpdateArea(short id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest("Area id does not match");
            }


            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            _context.Entry(area).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AreaExists(id))
            {
                return NotFound("Area not found");
            }

            return NoContent();
        }
        //Assign cage to an area
        [HttpPut]
        [Route("api/areas/{id}/cages")]
        public async Task<IActionResult> AssignCage(short id, Cage cage)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }

            if (_context.Cages == null)
            {
                return Problem("Entity set 'ZoomaContext.Cages'  is null.");
            }
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound("Area not found");
            }
            var c = _context.Cages.FindAsync(cage.Id);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AreaExists(id))
            {
                return NotFound("Area not found");
            }

            return NoContent();
        }

        public async Task<IActionResult> Index()
        {
              return _context.Areas != null ? 
                          View(await _context.Areas.ToListAsync()) :
                          Problem("Entity set 'ZoomaContext.Areas'  is null.");
        }

        // GET: Areas/Details/5

        public async Task<IActionResult> Details(short? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Areas/Create

        public IActionResult Create()
        {
            return View();
        }

        // POST: Areas/Create

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Status")] Area area)
        {
            if (ModelState.IsValid)
            {
                _context.Add(area);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(area);
        }

        // GET: Areas/Edit/5

        public async Task<IActionResult> Edit(short? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        // POST: Areas/Edit/5

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(short id, [Bind("Id,Name,Description,Status")] Area area)
        {
            if (id != area.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(area);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AreaExists(area.Id))
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
            return View(area);
        }

        // GET: Areas/Delete/5

        public async Task<IActionResult> Delete(short? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // POST: Areas/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(short id)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                _context.Areas.Remove(area);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AreaExists(short id)
        {
          return (_context.Areas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    } 
}

