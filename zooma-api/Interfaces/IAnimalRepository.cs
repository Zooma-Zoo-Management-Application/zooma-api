using zooma_api.DTO;
using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface IAnimalRepository
    {
        List<Animal> GetAllAnimals();
        Animal GetAnimalById(int id);
        List<Animal> GetAnimalByName(string name);
        List<Animal> GetAnimalsByCageId(int id);
        List<Animal> GetAnimalsByAreaId(int id);
        List<Animal> GetAllAnimalsWithNoCage();
        bool UpdateAnimalInCage();
        bool AssignAnimalToACage(int cageId, int animalId);
        bool UnassignAnimal(int animalId);
        bool Save();
    }
}
