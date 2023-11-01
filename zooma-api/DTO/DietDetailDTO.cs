using System.Text.Json.Serialization;

namespace zooma_api.DTO
{
    public class DietDetailDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime? ScheduleAt { get; set; }
        public DateTime? EndAt { get; set; }
        [JsonIgnore]
        public string? FeedingDate { get; set; }
        public TimeSpan? FeedingTime { get; set; }
        public double? Quantity { get; set; }
        public bool Status { get; set; }
        public int DietId { get; set; }
        public int FoodId { get; set; }
        public string[] FeedingDateArray { get; set; }
        public DietDTO diet { get; set; }
        public FoodDTO food { get; set; }   
    }
}
