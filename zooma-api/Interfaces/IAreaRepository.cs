using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IAreaRepository
    {
        List<Area> GetAllAreas();
        Area GetAreaById(short id);
        short[] GetAreaBySpeciesId(short id);
    }
}
