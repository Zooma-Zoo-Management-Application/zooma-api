namespace zooma_api.DTO
{
    public class DietDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime ScheduleAt { get; set; }
        public bool Status { get; set; }
        public string Goal { get; set; } = null!;
        public DateTime EndAt { get; set; }
        public double TotalEnergyValue { get; set; }
    }
}
