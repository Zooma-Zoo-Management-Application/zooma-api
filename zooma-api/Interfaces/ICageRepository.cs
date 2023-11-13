using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface ICageRepository
    {
        List<Cage> GetAllCages();
        Cage GetCageById(int id);
        List<Cage> GetCageByAreaId(int areaId);
        List<Cage> GetCageIsNotFullByAreaId(int areaId);
    }
}
