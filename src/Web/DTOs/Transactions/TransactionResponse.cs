namespace Web.DTOs.Transactions
{
    public class TransactionResponse
    {
        public decimal Amount { get; set; }
        public string Note { get; set; } = string.Empty;
        public DateTime Date { get; set; } 
    }
}
