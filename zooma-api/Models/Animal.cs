using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Animal
    {
        public Animal()
        {
            AnimalUsers = new HashSet<AnimalUser>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime DateOfBirth { get; set; }
        public float? Height { get; set; }
        public float? Weight { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public int SpieciesId { get; set; }
        public int? DietId { get; set; }
        public short? CageId { get; set; }
        public short? TrainingPlanId { get; set; }
        public string? ImageUrl { get; set; }
        public double MinRer { get; set; }
        public double MaxRer { get; set; }

        public virtual Cage? Cage { get; set; }
        public virtual Diet? Diet { get; set; }
        public virtual Species Spiecies { get; set; } = null!;
        public virtual TrainingPlan? TrainingPlan { get; set; }
        public virtual ICollection<AnimalUser> AnimalUsers { get; set; }
    }
}
