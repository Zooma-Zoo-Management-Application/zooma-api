using zooma_api.Models;

namespace zooma_api.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }
        public DateTime TicketDate { get; set; }
        public byte Quantity { get; set; }
        public byte UsedTicket { get; set; }
        public int OrderId { get; set; }
        public int TicketId { get; set; }

        public TicketDTO Ticket { get; set; } = null!;

    }
}
