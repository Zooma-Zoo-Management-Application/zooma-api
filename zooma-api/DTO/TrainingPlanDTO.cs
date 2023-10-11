namespace zooma_api.DTO
{
    public class TrainingPlanDTO
    {
        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string TrainingGoal { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }
    }
}
