using zooma_api.Models;

namespace zooma_api.DTO
{
    public class TrainerExpDTO
    {
        public int Id { get; set; }
        public byte YearOfExperience { get; set; }
        public string Description { get; set; }
        public byte Status { get; set; }
        public short UserId { get; set; }
        public short SkillId { get; set; }

        public SkillDTO Skill { get; set; }
        public UserDTO User { get; set; }
    }
}
