using System;
using System.Collections.Generic;

namespace zooma_api.Models
{
    public partial class OrderDetail
    {
        public int Id { get; set; }
        public DateTime TicketDate { get; set; }
        public byte Quantity { get; set; }
        public byte UsedTicket { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }

        public virtual Order Order { get; set; }
        public virtual Ticket Ticket { get; set; }
    }
}
