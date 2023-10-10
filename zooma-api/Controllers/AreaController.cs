using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    public class AreaController : Controller
    {
        private readonly zoomadbContext _context;

        public AreaController(zoomadbContext context)
        {
            _context = context;
        }
        //GET: GetAllAreas
        [HttpGet]
        [Route("GetAllAreas")]
        public async Task<IActionResult> GetAllAreas()
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            return Ok(await _context.Areas.ToListAsync());
        }
        //POST: AddArea
        [HttpPost]
        [Route("AddArea")]
        public async Task<IActionResult> AddArea([FromBody] Area area)
        {
            if (area == null)
            {
                return BadRequest("No area available");
            }
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return Ok(area);
        }
        //PUT: UpdateArea
        [HttpPut]
        [Route("UpdateArea")]
        public async Task<IActionResult> UpdateArea([FromBody] Area area)
        {
            if (area == null)
            {
                return BadRequest("No area available");
            }
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            var existingArea = await _context.Areas.FirstOrDefaultAsync(a => a.Id == area.Id);
            if (existingArea == null)
            {
                return NotFound("Area not found");
            }
            existingArea.Name = area.Name;
            existingArea.Description = area.Description;
            existingArea.Status = area.Status;
            await _context.SaveChangesAsync();
            return Ok(existingArea);
        }
        //GET: GetCages from CageController
        [HttpGet]
        [Route("GetCages")]
        public async Task<IActionResult> GetCages()
        {
            if (_context.Cages == null)
            {
                return Problem("Entity set 'ZoomaContext.Cages'  is null.");
            }
            return Ok(await _context.Cages.ToListAsync());
        }
        //PUT: Assign Cage to Area
        [HttpPut]
        [Route("AssignCageToArea")]
        public async Task<IActionResult> AssignCageToArea([FromBody] AreaCage areaCage)
        {
            if (areaCage == null)
            {
                return BadRequest("No area cage available");
            }
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
           
            var existingArea = await _context.Areas.FirstOrDefaultAsync(a => a.Id == areaCage.AreaId);
            if (existingArea == null)
            {
                return NotFound("Area not found");
            }
            var existingCage = await _context.Cages.FirstOrDefaultAsync(c => c.Id == areaCage.CageId);
            if (existingCage == null)
            {
                return NotFound("Cage not found");
            }
            existingArea.Cage = existingCage;
            _context.Entry(existingArea).State = EntityState.Modified; // tao thong bao thay doi trang thai vi update

            await _context.SaveChangesAsync();
            return Ok(existingArea);
            
        }

        


        // GET: Area
        public async Task<IActionResult> Index()
        {
              return _context.Areas != null ? 
                          View(await _context.Areas.ToListAsync()) :
                          Problem("Entity set 'ZoomaContext.Areas'  is null.");
        }

        // GET: Area/Details/5
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

        // GET: Area/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Area/Create
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

        // GET: Area/Edit/5
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

        // POST: Area/Edit/5
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

        // GET: Area/Delete/5
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

        // POST: Area/Delete/5
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
