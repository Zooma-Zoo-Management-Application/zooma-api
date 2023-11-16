using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class AreaRepository : IAreaRepository
    {
        public List<Area> GetAllAreas()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var area = _context.Areas.ToList();

                    return area;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Area GetAreaById(short id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var area = _context.Areas.FirstOrDefault(e => e.Id == id);

                    return area;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public short[] GetAreaBySpeciesId(short id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var cageOfSpecies = _context.Animals.Where(e => e.SpeciesId == id)
                                                        .Select(e => e.CageId)
                                                        .Distinct()
                                                        .ToList();

                    List<short?> cageId = cageOfSpecies.Where(e => e!=null).ToList();
                    List<short> areaIds = new List<short>();

                    foreach(var item in cageId)
                    {
                        var area = _context.Cages.Where(e => e.Id == item)
                                                 .Select(a => a.AreaId)
                                                 .FirstOrDefault();

                        if(area != null)
                        {
                            areaIds.Add(area);
                        }
                    }

                    short[] uniqueAreaIds = areaIds.Distinct().ToArray();
                    return uniqueAreaIds;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
