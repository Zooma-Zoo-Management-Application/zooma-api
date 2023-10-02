namespace zooma_api.Controllers
{
    public class TransactionDTO
    {
        public DateTime Date { get; set; }
        public string? AccountNumber { get; set; }
        public string? TransactionToken { get; set; }
        public double AmountOfMoney { get; set; }
        public bool Status { get; set; }
        public int OrderId { get; set; }
    }
}
