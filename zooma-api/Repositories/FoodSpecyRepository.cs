using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class FoodSpecyRepository : IFoodSpecyRepository
    {
        public List<FoodSpecy> GetAllFoodSpecy()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var foodSpecy = _context.FoodSpecies.
                                                          Include(e => e.Species).
                                                          Include(e => e.Food).
                                                          ToList();
                    return foodSpecy;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public FoodSpecy GetFoodSpecyById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var foodSpecy = _context.FoodSpecies.
                                                          Include(e => e.Species).
                                                          Include(e => e.Food).
                                                          FirstOrDefault(e => e.Id == id);
                    return foodSpecy;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
