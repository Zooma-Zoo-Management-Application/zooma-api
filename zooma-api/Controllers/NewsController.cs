using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        public ZoomaContext _context = new ZoomaContext();
        private readonly IMapper _mapper;

        public NewsController(IMapper mapper)
        {
            _mapper = mapper;
        }

        // Hàm lấy tất cả News
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<NewsDTO>>> GetAllNews()
        {
          if (_context.News == null)
          {
              return NotFound();
          }

            var news = await _context.News.Include(n => n.User).ToListAsync();

            var newsDTO = _mapper.Map<ICollection<NewsDTO>>(news);

            return Ok(newsDTO);
        }

        // Hàm lấy News dựa trên Id
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDTO>> GetNewsById(short id)
        {
            var news = await _context.News.Include(n => n.User).FirstOrDefaultAsync(n => n.Id == id);

            if (news == null)
            {
                return NotFound();
            }

            var newsDTO = _mapper.Map<NewsDTO>(news);

            return Ok(newsDTO);
        }

        // Hàm lấy News dựa trên staffid
        [HttpGet("/Staffs/{staffId}")]
        public async Task<ActionResult<NewsDTO>> GetNewsByStaffId(short staffId)
        {
            var user = await _context.Users.Include(n => n.News).FirstOrDefaultAsync(n => n.Id == staffId);

            if (user == null)
            {
                return NotFound();
            }

            var newsDTO = _mapper.Map<List<NewsDTO>>(user.News);

            return Ok(newsDTO);
        }

        //Hàm lấy các pinned news
        [HttpGet("pin-news")]
        public async Task<ActionResult<NewsDTO>> GetPinNews()
        {
            if (_context.News == null)
            {
                return NotFound();
            }

            List<NewsDTO> pinnedNews = _mapper.Map<List<NewsDTO>>(await _context.News.Where(a => a.Status == true).ToListAsync());

            if (pinnedNews == null)
            {
                return NotFound();
            }

            return Ok(pinnedNews);
        }

        //Hàm lấy các unpinned news
        [HttpGet("unpin-news")]
        public async Task<ActionResult<NewsDTO>> GetUnpinNews()
        {
            if (_context.News == null)
            {
                return NotFound();
            }

            List<NewsDTO> unpinNews = _mapper.Map<List<NewsDTO>>(await _context.News.Where(a => a.Status == false).ToListAsync());

            if (unpinNews == null)
            {
                return NotFound();
            }

            return Ok(unpinNews);
        }

        // PUT: api/News/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(short id, [FromBody]NewsBody newsBody)
        {
            var existingNews = await _context.News.FirstOrDefaultAsync(e => e.Id == id);

            if (existingNews == null)
            {
                return NotFound();
            }

            _mapper.Map(newsBody, existingNews);

            _context.Entry(existingNews).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // Hàm tạo news 
        [HttpPost()]
        public async Task<ActionResult<News>> CreateNews(NewsBody newsBody)
        {
          if (_context.News == null)
          {
              return Problem("Entity set 'ZoomaContext.News'  is null.");
          }
            var news = _mapper.Map<News>(newsBody);
            _context.News.Add(news);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetNews", new { id = news.Id }, news);
        }

        //Hàm Pin News

        [HttpPut("{id}/pin-news")]
        public async Task<IActionResult> PinNews(short id)
            {

            var existingNews = await _context.News.FindAsync(id);

            if (existingNews == null)
            {
                return NotFound();
            }

            if(existingNews.Status != false)
            {
                return BadRequest(existingNews);
            }

            existingNews.Status = true;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        [HttpPut("{id}/unpin-news")]
        public async Task<IActionResult> UnpinNews(short id)
        {

            var existingNews = await _context.News.FindAsync(id);

            if (existingNews == null)
            {
                return NotFound();
            }

            if (existingNews.Status != true)
            {
                return BadRequest(existingNews);
            }

            existingNews.Status = false;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!NewsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }



        // DELETE: api/News/5
              [HttpDelete("{id}")]
              public async Task<IActionResult> DeleteNews(short id)
              {
                  if (_context.News == null)
                  {
                      return NotFound();
                  }
                  var news = await _context.News.FindAsync(id);
                  if (news == null)
                  {
                      return NotFound();
                  }

                  _context.News.Remove(news);
                  await _context.SaveChangesAsync();

                  return NoContent();
              } 

        private bool NewsExists(short id)
        {
            return (_context.News?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        
    }
}
