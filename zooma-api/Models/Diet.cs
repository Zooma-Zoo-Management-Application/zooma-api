using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Diet
    {
        public Diet()
        {
            Animals = new HashSet<Animal>();
            DietDetails = new HashSet<DietDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; }
        public DateTime ScheduleAt { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<DietDetail> DietDetails { get; set; }
    }
}
