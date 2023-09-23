using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class Transaction
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? AccountNumber { get; set; }
        public string? TransactionToken { get; set; }
        public double AmountOfMoney { get; set; }
        public bool Status { get; set; }
        public int OrderId { get; set; }

        public virtual Order Order { get; set; } = null!;
    }
}
