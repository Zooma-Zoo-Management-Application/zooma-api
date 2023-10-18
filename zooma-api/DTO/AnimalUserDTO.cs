using zooma_api.Models;

namespace zooma_api.DTO
{
    public class AnimalUserDTO
    {
        public int AnimalId { get; set; }
        public short UserId { get; set; }
        public bool MainTrainer { get; set; }
        public AnimalWithCage Animal { get; set; } 
        public UserDTO User { get; set; }
    }
}
