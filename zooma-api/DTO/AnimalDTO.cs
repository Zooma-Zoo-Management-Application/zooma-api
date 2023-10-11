using zooma_api.Models;

namespace zooma_api.DTO
{
    public class AnimalDTO
    {
        public int? Id { get; set; }
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

        public DietDTO? diet { get; set; }
        public CagesDTO? cage { get; set; }
        public SpeciesDTO? species { get; set; }
        public TrainingPlanDTO? trainingPlan { get; set; }

    }
}
