using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class AnimalUser
    {
        public int AnimalId { get; set; }
        public short UserId { get; set; }
        public bool MainTrainer { get; set; }
        public virtual Animal Animal { get; set; }
        public virtual User User { get; set; }
    }
}
