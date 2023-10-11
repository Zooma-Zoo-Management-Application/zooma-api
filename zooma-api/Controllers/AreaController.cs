using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        //get all
        [HttpGet]
        [Route("api/GetAllAreas")]
        public async Task<ActionResult<IEnumerable<Area>>> GetAllAreas()
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            return Ok(_context.Areas);
        }
        //get area by id
        [HttpGet]
        [Route("api/GetAreaById/{id}")]
        public async Task<ActionResult<Area>> GetAreaById(short id)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound("No area having that ID");
            }
            return Ok(area);
        }
        //create area
        [HttpPost]
        [Route("api/CreateArea")]
        public async Task<ActionResult<Area>> CreateArea(Area area)
        {
            if (_context.Areas == null)
            {
                return NotFound("No area available");
            }
            _context.Areas.Add(area);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetAreaById", new { id = area.Id }, area);
        }
        //update area
        [HttpPut]
        [Route("api/UpdateArea/{id}")]
        public async Task<IActionResult> UpdateArea(short id, Area area)
        {
            if (id != area.Id)
            {
                return BadRequest();
            }
            _context.Entry(area).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
                Console.WriteLine("Update success");
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(id))
                {
                    return NotFound("Area not exists");
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }


        // GET: Area
        public async Task<IActionResult> Index()
        {
            return _context.Areas != null ?
                        View(await _context.Areas.ToListAsync()) :
                        Problem("Entity set 'zoomadbContext.Areas'  is null.");
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
                return Problem("Entity set 'zoomadbContext.Areas'  is null.");
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
