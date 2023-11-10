using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class DietRepository : IDietRepository
    {
        public List<Diet> GetAllDiets()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var diet = _context.Diets.Include(a => a.Animals).ToList();

                    return diet;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Diet GetDietById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var diet = _context.Diets.Include(a => a.Animals).FirstOrDefault(e => e.Id == id);

                    return diet;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Diet GetDietByName(string name)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var diet = _context.Diets.SingleOrDefault(d => d.Name == name);

                    return diet;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
