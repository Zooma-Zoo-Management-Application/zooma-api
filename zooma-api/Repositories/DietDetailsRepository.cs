using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class DietDetailsRepository : IDietDetailsRepository
    {
        public List<DietDetail> GetAllDietDetails()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var dietDetails = _context.DietDetails.
                                                           Include(e => e.Diet).
                                                           Include(e => e.Food).
                                                           ToList();

                    return dietDetails;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<DietDetail> GetDietDetailByDietId(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var dietDetail = _context.DietDetails.
                                                          Include(e => e.Diet).
                                                          Include(e => e.Food).
                                                          Where(e => e.DietId == id).
                                                          ToList();

                    return dietDetail;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public DietDetail GetDietDetailById(int dietId)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var dietDetail = _context.DietDetails.
                                                           Include(e => e.Diet).
                                                           Include(e => e.Food).
                                                           FirstOrDefault(e => e.Id == dietId);

                    return dietDetail;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
