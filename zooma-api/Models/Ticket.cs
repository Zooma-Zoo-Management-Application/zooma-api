using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Ticket
    {
        public Ticket()
        {
            OrderDetails = new HashSet<OrderDetail>();
        }

        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public float Price { get; set; }
        public string? Description { get; set; }
        public bool? Status { get; set; }

        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}
