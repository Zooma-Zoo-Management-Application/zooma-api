using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class TrainingPlan
    {
        public TrainingPlan()
        {
            Animals = new HashSet<Animal>();
            TrainingDetails = new HashSet<TrainingDetail>();
        }

        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string TrainingGoal { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<TrainingDetail> TrainingDetails { get; set; }
    }
}
