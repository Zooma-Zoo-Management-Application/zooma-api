using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Type
    {
        public Type()
        {
            Species = new HashSet<Species>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageUrl { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<Species> Species { get; set; }
    }
}
