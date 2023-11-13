using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IDietRepository
    {
        List<Diet> GetAllDiets();
        Diet GetDietById(int id);
        Diet GetDietByName(string name);
        List<Diet> GetInActiveDiet();
        Double CountEnergyOfDiet(int dietId);
    }
}
