namespace zooma_api.DTO
{
    public class AnimalUpdate
    {
        public string? Name { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }
        public int? SpieciesId { get; set; }
        public int? DietId { get; set; }
        public short? CageId { get; set; }
        public short? TrainingPlanId { get; set; }
    }
}
