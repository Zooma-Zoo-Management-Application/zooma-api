using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Cage
    {
        public Cage()
        {
            Animal = new HashSet<Animal>();
        }

        public short Id { get; set; }
        public string Name { get; set; } = null!;
        public byte AnimalLimit { get; set; }
        public byte AnimalCount { get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; }
        public short AreaId { get; set; }

        public virtual Area Area { get; set; } = null!;
        public virtual ICollection<Animal> Animal { get; set; }
    }
}
