namespace zooma_api.DTO
{
    public class CartItemDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public byte quantity { get; set; }
        public string? Description { get; set; }

        public DateTime TicketDate { get; set; }

    }
}
