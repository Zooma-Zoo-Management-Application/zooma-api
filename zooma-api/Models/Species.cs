using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Species
    {
        public Species()
        {
            Animals = new HashSet<Animal>();
            FoodSpecies = new HashSet<FoodSpecy>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public bool Status { get; set; }
        public int? TypeId { get; set; }

        public virtual Type? Type { get; set; }
        public virtual ICollection<Animal> Animals { get; set; }
        public virtual ICollection<FoodSpecy> FoodSpecies { get; set; }
    }
}
