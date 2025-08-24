namespace Web.DTOs.Transactions
{
    public class TransactionRequest
    {
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
    }
}
