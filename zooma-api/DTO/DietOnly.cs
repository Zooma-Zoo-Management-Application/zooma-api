namespace zooma_api.DTO
{
    public class DietOnly
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime ScheduleAt { get; set; }
        public string Goal { get; set; }
        public DateTime EndAt { get; set; }
        public double TotalEnergyValue { get; set; }
        public bool Status { get; set; }
    }
}
