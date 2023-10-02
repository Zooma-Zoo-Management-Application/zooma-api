using zooma_api.Models;

namespace zooma_api.DTO
{
    public class AreasDTO
    {
        public AreasDTO(Area a)
        {
            A = a;
        }

        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }
        public Area A { get; }
    }
}

