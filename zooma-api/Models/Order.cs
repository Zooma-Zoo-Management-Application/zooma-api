using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Order
    {
        public Order()
        {
            OrderDetails = new HashSet<OrderDetail>();
            Transactions = new HashSet<Transaction>();
        }

        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public bool Status { get; set; }
        public short UserId { get; set; }
        public DateTime? LastUpdateDate { get; set; }

        public virtual User User { get; set; } = null!;
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
        public virtual ICollection<Transaction> Transactions { get; set; }
    }
}
