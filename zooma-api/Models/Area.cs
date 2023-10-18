using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Area
    {
        public Area()
        {
            Cages = new HashSet<Cage>();
        }

        public short Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Cage> Cages { get; set; }
    }
}
