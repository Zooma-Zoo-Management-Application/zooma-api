namespace zooma_api.DTO
{
    public class OrderInfo
    {
        public long OrderId { get; set; }
        public float Amount { get; set; }
        public string OrderDesc { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
