using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class NewRepository : INewRepository
    {
        public List<News> GetAllNews()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var news = _context.News.Include(a => a.User).ToList();  

                    return news;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public News GetNewById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var news = _context.News.Include(a => a.User).FirstOrDefault(e => e.Id == id);

                    return news;
                }

                catch (Exception)
                {

                    throw;
                }
            }
        }

        public User GetNewsByStaffId(short id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var news = _context.Users.Include(n => n.News).FirstOrDefault (n => n.Id == id);

                    return news;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<News> GetPinNews()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var news = _context.News.Where(e => e.Status == true).ToList(); 

                    return news;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<News> GetUnpinNews()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var news = _context.News.Where(e => e.Status == false).ToList();

                    return news;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
