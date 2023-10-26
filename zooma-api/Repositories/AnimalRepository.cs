using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        public List<Animal> GetAllAnimals()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var animals = _context.Animals.
                                                  Include(n => n.Diet).
                                                  Include(n => n.Cage).
                                                  Include(n => n.Species).
                                                  ToList();
                    return animals;
                } 
                catch(Exception)
                {
                    throw;
                }

            }
        }

        public List<Animal> GetAllAnimalsWithNoCage()
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var animal = _context.Animals.Where(e => e.CageId == null).ToList();

                    return animal;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Animal GetAnimalById(int id)
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var animal = _context.Animals.
                                                  Include(n => n.TrainingPlan).
                                                  Include(n => n.Diet).
                                                  Include(n => n.Species).
                                                  Include(n => n.Cage).
                                                  FirstOrDefault(e => e.Id == id);

                    return animal;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Animal> GetAnimalByName(string name)
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    List<Animal> animals = new();

                    if (name == null)
                    {
                        animals = _context.Animals.Include(n => n.Diet).
                                                   Include(n => n.Species).
                                                   Include(n => n.Cage).
                                                   ToList();
                        return animals;
                    }
                    else
                    {
                        animals = _context.Animals.Where(a => a.Name.Contains(name)).
                                                                                     Include(n => n.TrainingPlan).
                                                                                     Include(n => n.Diet).
                                                                                     Include(n => n.Species).
                                                                                     Include(n => n.Cage).ToList();

                        return animals;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Animal> GetAnimalsByAreaId(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var cages = _context.Cages.Where(a => a.AreaId == id).Select(a => a.Id).ToList();

                    var animals = _context.Animals.
                                                    Where(a => cages.Contains((short)a.CageId)).
                                                    Include(n => n.Diet).
                                                    Include(n => n.Species).
                                                    Include(n => n.Cage).
                                                    ToList();
                    return animals;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public List<Animal> GetAnimalsByCageId(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var animals = _context.Animals.Where(c => c.CageId == id).
                                                           Include(n => n.Diet).
                                                           Include(n => n.Species).
                                                           Include(n => n.Cage).
                                                           ToList();
                    return animals;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
