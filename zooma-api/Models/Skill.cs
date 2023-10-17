using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Skill
    {
        public Skill()
        {
            TrainerExps = new HashSet<TrainerExp>();
            TrainingDetails = new HashSet<TrainingDetail>();
        }

        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<TrainerExp> TrainerExps { get; set; }

        public virtual ICollection<TrainingDetail> TrainingDetails { get; set; }
    }
}
