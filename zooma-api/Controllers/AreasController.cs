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
    public class AreasController : Controller
    {
        private readonly ZoomaContext _context;

        public AreasController(ZoomaContext context)
        {
            _context = context;
        }
        
        //hàm này để lấy danh sách các khu vực đã có trong hệ thống
        [HttpGet]
        [Route("api/areas")]
        public async Task<ActionResult<IEnumerable<Area>>> GetAreas()
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            return await _context.Areas.ToListAsync();
        }

        //get detailed information of an area
        [HttpGet]
        [Route("api/areas/{id}")]
        public async Task<ActionResult<AreasDTO>> GetArea(byte id)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            var area = await _context.Areas.FindAsync(id);

            if (area == null)
            {
                return NotFound();
            }

            return new AreasDTO(area);
        }
        //create a new area
        //[HttpPost]
        //[Route("api/areas")]
        //public async Task<ActionResult<AreasDTO>> PostArea(AreasDTO areaDTO)
        //{
        //    if (_context.Areas == null)
        //    {
        //        return Problem("Entity set 'ZoomaContext.Areas'  is null.");
        //    }
        //    //var area = new Area(areaDTO);
        //    //_context.Areas.Add(area);
        //    await _context.SaveChangesAsync();
//
            //return CreatedAtAction(nameof(GetArea), new { id = area.Id }, new AreasDTO(area));
        //}
        //update an area
        [HttpPut]
        [Route("api/areas/{id}")]
        public async Task<IActionResult> PutArea(byte id, AreasDTO areaDTO)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ZoomaContext.Areas'  is null.");
            }
            if (id != areaDTO.Id)
            {
                return BadRequest();
            }
            //var area = new Area(areaDTO);
            //_context.Entry(area).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException) when (!AreaExists(id))
            {
                return NotFound();
            }

            return NoContent();
        }
        //assign cage to the area
        [HttpPost]
        [Route("api/areas/{id}/cages")]
        public async Task<IActionResult> PostCageToArea(byte id, CagesDTO cageDTO)
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
            var cage = await _context.Cages.FindAsync(cageDTO.Id);
            if (cage == null)
            {
                return NotFound("Cage not found");
            }
            area.Cages.Add(cage);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArea), new { id = area.Id }, new AreasDTO(area));
           
        }
        //remove cage from the area
        [HttpDelete]
        [Route("api/areas/{id}/cages")]
        public async Task<IActionResult> DeleteCageFromArea(byte id, CagesDTO cageDTO)
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
            var cage = await _context.Cages.FindAsync(cageDTO.Id);
            if (cage == null)
            {
                return NotFound("Cage not found");
            }
            area.Cages.Remove(cage);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(GetArea), new { id = area.Id }, new AreasDTO(area));
           
            
        }

        // GET: Areas
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