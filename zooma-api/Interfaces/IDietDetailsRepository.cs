using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IDietDetailsRepository
    {
        List<DietDetail> GetAllDietDetails();
        DietDetail GetDietDetailById(int dietId);
        List<DietDetail> GetDietDetailByDietId(int id);
    }
}
