using zooma_api.Interfaces;
using zooma_api.Models;

namespace zooma_api.Repositories
{
    public class SkillRepository : ISkillRepository
    {
        public List<Skill> GetAllSkills()
        {
            using(var _context = new zoomadbContext())
            {
                try
                {
                    var skill = _context.Skills.ToList();
                    return skill;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }

        public Skill GetSkillById(int skillId)
        {
            using (var _context = new zoomadbContext())
            {
                try
                {
                    var skill = _context.Skills.FirstOrDefault(e => e.Id == skillId);

                    return skill;
                }
                catch (Exception)
                {

                    throw;
                }
            }
        }
    }
}
