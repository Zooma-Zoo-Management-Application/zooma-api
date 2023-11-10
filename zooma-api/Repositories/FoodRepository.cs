using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class FoodRepository : IFoodRepository
    {
        public List<Food> GetAllFoods()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var food = _context.Foods.Include(a => a.FoodSpecies).ToList();

                    return food;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Food GetById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var food = _context.Foods.FirstOrDefault(e => e.Id == id);

                    return food;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
