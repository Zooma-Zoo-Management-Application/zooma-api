using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class FoodSpecy
    {
        public int Id { get; set; }
        public int SpeciesId { get; set; }
        public int FoodId { get; set; }
        public bool CompatibilityStatus { get; set; }

        public virtual Food Food { get; set; } = null!;
        public virtual Species Species { get; set; } = null!;
    }
}
