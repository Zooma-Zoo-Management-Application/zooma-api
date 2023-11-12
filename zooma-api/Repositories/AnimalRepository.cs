using Microsoft.EntityFrameworkCore;
using zooma_api.DTO;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class AnimalRepository : IAnimalRepository
    {
        public bool AssignAnimalToACage(int cageId, int animalId)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var cage = _context.Cages.FirstOrDefault(e => e.Id == cageId);
                    var animal = _context.Animals.FirstOrDefault(e => e.Id == animalId);
                    var animalInCage = _context.Animals.Count(e => e.CageId == cageId);

                    if (animalInCage == cage.AnimalLimit)
                    {
                        return false;
                    }
                    else
                    {
                        animal.CageId = (short?)cageId;
                        cage.AnimalCount = (byte)(animalInCage + 1);
                        _context.Entry(cage).State = EntityState.Modified;
                        _context.Entry(animal).State = EntityState.Modified;
                        return _context.SaveChanges() > 0;
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

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

        public bool Save()
        {
            using(var _context = new zoomadbContext())
            {
                var saved = _context.SaveChanges();
                return saved > 0 ? true : false;
            }
        }

        public bool UnassignAnimal(int animalId)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var animal = _context.Animals.FirstOrDefault(e => e.Id == animalId);
                    var cage = _context.Cages.FirstOrDefault(e => e.Id == animal.CageId);
                    
                    if(animal == null)
                    {
                        return false;
                    }
                    else
                    {
                        if(animal.CageId != null)
                        {
                            animal.CageId = null;
                            var animalInCage = _context.Animals.Count(e => e.CageId == animal.CageId);

                            animalInCage--;

                            cage.AnimalCount = (byte)animalInCage;

                            _context.Entry(cage).State = EntityState.Modified;
                            _context.Entry(animal).State = EntityState.Modified;

                            return _context.SaveChanges()>0;
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        public bool UpdateAnimalInCage()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    bool status = true;
                    var cage = _context.Cages.ToList();
                    foreach (var c in cage)
                    {
                        var animalOfCage = _context.Animals.Count(e => e.CageId == c.Id);
                        c.AnimalCount = (byte)animalOfCage;
                        if (c.AnimalCount > c.AnimalLimit)
                        {
                            status = false;
                            break;
                        }
                    }

                    if (status == true)
                    {
                        return Save();
                    }
                    else
                    {
                        return false;
                    }
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
