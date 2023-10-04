namespace zooma_api.DTO
{
    public class TicketDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public string? Description { get; set; }
    }
}
