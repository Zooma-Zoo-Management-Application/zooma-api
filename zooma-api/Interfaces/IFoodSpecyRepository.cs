using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IFoodSpecyRepository
    {
        List<FoodSpecy> GetAllFoodSpecy();
        FoodSpecy GetFoodSpecyById(int id);
    }
}
