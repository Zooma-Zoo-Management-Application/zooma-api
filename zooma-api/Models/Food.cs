using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Food
    {
        public Food()
        {
            DietDetails = new HashSet<DietDetail>();
            FoodSpecies = new HashSet<FoodSpecy>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public double EnergyValue { get; set; }
        public string ImageUrl { get; set; }
        public bool Status { get; set; }

        public virtual ICollection<DietDetail> DietDetails { get; set; }
        public virtual ICollection<FoodSpecy> FoodSpecies { get; set; }
    }
}
