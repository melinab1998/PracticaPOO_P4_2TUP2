namespace Web.DTOs.Transactions
{
    public class BalanceResponse
    {
        public string Owner { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
