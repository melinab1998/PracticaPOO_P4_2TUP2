namespace Web.DTOs.Accounts
{
    public class BalanceResponse
    {
        public string Owner { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
