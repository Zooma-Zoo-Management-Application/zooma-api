using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class TrainingDetail
    {
        public int Id { get; set; }
        public int? Column { get; set; }
        public short TrainingPlanId { get; set; }

        public virtual TrainingPlan TrainingPlan { get; set; } = null!;
    }
}
