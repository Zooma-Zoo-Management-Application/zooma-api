using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class TrainingDetail
    {
        public TrainingDetail()
        {
            Skills = new HashSet<Skill>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? Status { get; set; }
        public short TrainingPlanId { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; }

        public virtual ICollection<Skill> Skills { get; set; }
    }
}
