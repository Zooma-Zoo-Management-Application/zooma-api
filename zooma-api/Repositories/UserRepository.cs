using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class UserRepository : IUserRepository
    {
        public async Task<AllUsersQuantity> GetUsersQuantityAsync() // PARALEL ASYNC METHOD
        {
           List<Task<int>> tasks = new List<Task<int>>();
            tasks.Add(Task.Run(() => getStaffsQuantity()));
            tasks.Add(Task.Run(() => getZooTrainersQuantity()));
            tasks.Add(Task.Run(() => getVisitorsQuantity()));

            var result = await Task.WhenAll(tasks);

            return new AllUsersQuantity
            {
                Staffs = tasks[0].Result,
                ZooTrainers = tasks[1].Result,
                Visitors = tasks[2].Result
            };
         

        }

        public int getStaffsQuantity()
        {
            using (var context = new zoomadbContext())
            {
                try
                {
                    var list = context.Users.Where(u => u.RoleId == 1).ToList();
                    return list.Count;

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public int getZooTrainersQuantity()
        {
            using (var context = new zoomadbContext())
            {
                try
                {
                    var list = context.Users.Where(u => u.RoleId == 2).ToList();
                    return list.Count;

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public int getVisitorsQuantity()
        {
            using (var context = new zoomadbContext())
            {
                try
                {
                    var list = context.Users.Where(u => u.RoleId == 3).ToList();
                    return list.Count;

                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }

    public class AllUsersQuantity
    {
        public int Staffs { get; set; }
        public int Visitors { get; set; }
        public int ZooTrainers { get; set; }
    }
}
