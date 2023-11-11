using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IZooTrainerRepository
    {
        List<User> GetAllZooTrainers();
        User GetZooTrainerById(int id);
    }
}
