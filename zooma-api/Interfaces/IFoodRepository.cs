using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IFoodRepository
    {
        List<Food> GetAllFoods();

        Food GetById(int id);
    }
}
