﻿namespace zooma_api.DTO
{
    public class AnimalWithCage
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int SpeciesId { get; set; }
        public int? DietId { get; set; }
        public short? CageId { get; set; }
        public short? TrainingPlanId { get; set; }
        public string? ImageUrl { get; set; }
        public double MinRer { get; set; }
        public double MaxRer { get; set; }
    }
}
