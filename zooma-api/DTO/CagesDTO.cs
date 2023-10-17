using zooma_api.Controllers;
using zooma_api.Models;

namespace zooma_api.DTO
{
    public class CagesDTO
    {
        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public byte AnimalLimit { get; set; }
        public byte AnimalCount { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public short AreaId { get; set; }

        public ICollection<AnimalWithCage> Animal { get; set; }
    }
}
