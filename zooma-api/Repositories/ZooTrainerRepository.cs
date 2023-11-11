using Microsoft.EntityFrameworkCore;
using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class ZooTrainerRepository : IZooTrainerRepository
    {
        public List<User> GetAllZooTrainers()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var zooTrainers = _context.Users.Where(a => a.RoleId == 2).ToList();
                    return zooTrainers;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public User GetZooTrainerById(int id)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var zooTrainer = _context.Users.FirstOrDefault(a => a.Id == id);
                    return zooTrainer;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
