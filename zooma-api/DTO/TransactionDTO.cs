namespace zooma_api.DTO
{
    public class TransactionDTO
    {
        public DateTime Date { get; set; }
        public string? AccountNumber { get; set; }
        public string? TransactionToken { get; set; }
        public double AmountOfMoney { get; set; }
        public bool Status { get; set; }
        public int OrderId { get; set; }
        public string? TransactionNo { get; set; }

    }
}
