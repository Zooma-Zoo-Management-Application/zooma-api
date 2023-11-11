using zooma_api.Models;

namespace zooma_api.Interfaces
{
    public interface ISkillRepository
    {
        List<Skill> GetAllSkills();
        Skill GetSkillById(int skillId);
    }
}
