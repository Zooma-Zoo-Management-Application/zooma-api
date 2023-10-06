using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    public class DietsController : Controller
    {
        private readonly ZoomaContext _context;

        public DietsController(ZoomaContext context)
        {
            _context = context;
        }
        // GET: Diets/GetAll
        [HttpGet("GetAll")]
        public async Task<IActionResult> GetAll()
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'ZoomaContext.Diets'  is null.");
            }
            var diets = await _context.Diets.ToListAsync();
            if (diets == null)
            {
                return NotFound("No diet available");
            }
            return Ok(diets);
        }
        //create diet
        // POST: Diets/Create
        [HttpPost("CreateDiet")]
        public async Task<IActionResult> CreateDiet([FromBody] Diet diet)
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'ZoomaContext.Diets'  is null.");
            }
            if (ModelState.IsValid)
            {
                _context.Add(diet);
                await _context.SaveChangesAsync();
                return Ok(diet);
            }
            return BadRequest(diet);
        }
        

        // PUT: Diets/Edit/5
        [HttpPut("Update/{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] Diet diet)
        {
            if (id != diet.Id)
            {
                return NotFound("No diets with that ID");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietExists(diet.Id))
                    {
                        return NotFound("No diets available");
                    }
                    else
                    {
                        throw;
                    }
                }
                return Ok(diet);
            }
            return BadRequest(diet);
        }


        // GET: Diets
        public async Task<IActionResult> Index()
        {
              return _context.Diets != null ? 
                          View(await _context.Diets.ToListAsync()) :
                          Problem("Entity set 'ZoomaContext.Diets'  is null.");
        }

        // GET: Diets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Diets == null)
            {
                return NotFound();
            }

            var diet = await _context.Diets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diet == null)
            {
                return NotFound();
            }

            return View(diet);
        }

        // GET: Diets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Diets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,CreateAt,UpdateAt,ScheduleAt,Status,Goal,EndAt,MinRer,MaxRer")] Diet diet)
        {
            if (ModelState.IsValid)
            {
                _context.Add(diet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(diet);
        }

        // GET: Diets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Diets == null)
            {
                return NotFound();
            }

            var diet = await _context.Diets.FindAsync(id);
            if (diet == null)
            {
                return NotFound();
            }
            return View(diet);
        }

        // POST: Diets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,CreateAt,UpdateAt,ScheduleAt,Status,Goal,EndAt,MinRer,MaxRer")] Diet diet)
        {
            if (id != diet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(diet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DietExists(diet.Id))
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
            return View(diet);
        }

        // GET: Diets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Diets == null)
            {
                return NotFound();
            }

            var diet = await _context.Diets
                .FirstOrDefaultAsync(m => m.Id == id);
            if (diet == null)
            {
                return NotFound();
            }

            return View(diet);
        }

        // POST: Diets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Diets == null)
            {
                return Problem("Entity set 'ZoomaContext.Diets'  is null.");
            }
            var diet = await _context.Diets.FindAsync(id);
            if (diet != null)
            {
                _context.Diets.Remove(diet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DietExists(int id)
        {
          return (_context.Diets?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
