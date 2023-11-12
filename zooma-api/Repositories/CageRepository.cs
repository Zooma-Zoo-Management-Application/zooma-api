using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class CageRepository : ICageRepository
    {
        public List<Cage> GetAllCages()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var cage = _context.Cages.ToList();
                    return cage;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Cage> GetCageByAreaId(int areaId)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var cages = _context.Cages.
                                               Where(a => a.AreaId == areaId).
                                               Where(a => a.Status == true).
                                               Include(a => a.Animal).
                                               ToList();

                    return cages;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Cage GetCageById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var cage = _context.Cages.FirstOrDefault(c => c.Id == id);
                    return cage;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Cage> GetCageIsNotFullByAreaId(int areaId)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var cages = _context.Cages.
                                               Where(a => a.AreaId == areaId).
                                               Where(a => a.Status == true).
                                               Where(a => a.AnimalCount < a.AnimalLimit).
                                               Include(b => b.Animal).
                                               ToList();
                    return cages;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
