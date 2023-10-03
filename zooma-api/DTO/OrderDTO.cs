using zooma_api.Models;

namespace zooma_api.DTO
{
    public class OrderDTO
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public float TotalPrice { get; set; }
        public string? Notes { get; set; }
        public string PaymentMethod { get; set; } = null!;
        public bool Status { get; set; }
        public short UserId { get; set; }

        // trả về kèm thêm
        public UserDTO User { get; set; } = null!;
        public ICollection<OrderDetailDTO> OrderDetails { get; set; } = new List<OrderDetailDTO>();
        public  ICollection<TransactionDTO> Transactions { get; set; } = new List<TransactionDTO>();



    }
}
