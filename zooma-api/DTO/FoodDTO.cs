namespace zooma_api.DTO
{
    public class FoodDTO
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public double EnergyValue { get; set; }
        public string? ImageUrl { get; set; }
        public bool Status { get; set; }
    }
}
