namespace zooma_api.DTO
{
    public class SpeciesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
