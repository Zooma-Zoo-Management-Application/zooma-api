using zooma_api.Models;

namespace zooma_api.DTO
{
    public class AreaDTO
    {
        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public ICollection<Cage> Cages { get; set; }
    }
}
