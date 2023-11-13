using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Plugins;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Controllers
{
    [Route("api/news")]
    [ApiController]
    public class NewsController : ControllerBase
    {
        public zoomadbContext _context = new zoomadbContext();

        private readonly IMapper _mapper;
        private readonly INewRepository _newRepository;

        public NewsController(IMapper mapper, INewRepository newRepository)
        {
            _mapper = mapper;
            _newRepository = newRepository;
        }

        // Hàm lấy tất cả News
        [HttpGet()]
        public ActionResult<IEnumerable<NewsDTO>> GetAllNews()
        {
            if (_context.News == null)
            {
                return NotFound();
            }

            var news = _newRepository.GetAllNews();

            var newsDTO = _mapper.Map<ICollection<NewsDTO>>(news);

            return Ok(newsDTO);
        }

        // Hàm lấy News dựa trên Id
        [HttpGet("{id}")]
        public ActionResult<NewsDTO> GetNewsById(short id)
        {
            var news = _newRepository.GetNewById(id);   

            if (news == null)
            {
                return NotFound("Can't found this new");
            }

            var newsDTO = _mapper.Map<NewsDTO>(news);

            return Ok(newsDTO);
        }

        // Hàm lấy News dựa trên staffid
        [HttpGet("get-news-by-staffId/{staffId}")]
        public ActionResult<NewsDTO> GetNewsByStaffId(short staffId)
        {
            var user = _newRepository.GetNewsByStaffId(staffId);

            if (user == null)
            {
                return NotFound("Can't find this user");
            }

            if (user.RoleId != 1)
            {
                return BadRequest("This user is not allowed to manage news");
            }

            var newsDTO = _mapper.Map<List<NewsDTO>>(user.News);

            return Ok(newsDTO);
        }

        //Hàm lấy các pinned news
        [HttpGet("pin-news")]
        public ActionResult<NewsDTO> GetPinNews()
        {
            if (_context.News == null)
            {
                return NotFound();
            }

            var news = _newRepository.GetPinNews();

            List<NewsDTO> pinnedNews = _mapper.Map<List<NewsDTO>>(news).ToList();

            if (pinnedNews == null)
            {
                return NotFound();
            }

            return Ok(pinnedNews);
        }

        //Hàm lấy các unpinned news
        [HttpGet("unpin-news")]
        public ActionResult<NewsDTO> GetUnpinNews()
        {
            if (_context.News == null)
            {
                return NotFound();
            }

            var news = _newRepository.GetUnpinNews();

            List<NewsDTO> unpinNews = _mapper.Map<List<NewsDTO>>(news).ToList();

            if (unpinNews == null)
            {
                return NotFound();
            }

            return Ok(unpinNews);
        }

        // PUT: api/News/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateNews(short id, UpdateNewsBody newsBody)
        {
            var existingNews = await _context.News.FirstOrDefaultAsync(e => e.Id == id);

            if (existingNews == null)
            {
                return NotFound("Khong tim thay news nay");
            }

            existingNews.Title = newsBody.Title;
            existingNews.Content = newsBody.Content;
            existingNews.Image = newsBody.Image;
            existingNews.Date = DateTime.Now;
            existingNews.Description = newsBody.Description;

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
            return Ok("Update successfully!");
        }

        // Hàm tạo news 
        [HttpPost()]
        public async Task<ActionResult<NewsDTO>> CreateNews(CreateNewsBody newsBody)
        {
          if (_context.News == null)
          {
              return Problem("Entity set 'ZoomaContext.News'  is null.");
          }

            News news = new News
            {
                Title = newsBody.Title,
                Content = newsBody.Content,
                Date = DateTime.Now,
                Description = newsBody.Description,
                Image = newsBody.Image,
                Status = false,
                UserId = newsBody.UserId,
            };

            var user = _context.Users.FirstOrDefault(x => x.Id == news.UserId); 

            if(user.RoleId != 1)
            {
                return BadRequest("This user can't create news");
            }

            _context.News.Add(news);
            await _context.SaveChangesAsync();

            /*      var news = _mapper.Map<News>(newsBody);
                  news.Date = DateTime.Now;*/

            // return CreatedAtAction("GetNewsById", new { id = news.Id }, news);
            return Ok(new { newsDTO = _mapper.Map<NewsDTO>(news), message = "News created successfully"});
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
            return Ok(existingNews);
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
            return Ok(existingNews);
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

    public class UpdateNewsBody
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
    }

    public class CreateNewsBody
    {
        public string? Title { get; set; }
        public string? Content { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public short UserId { get; set; }
    }
}
