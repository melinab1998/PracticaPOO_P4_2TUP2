namespace Web.DTOs.Accounts
{
    public class BankAccountResponse
    {
        public string Number { get; set; } = string.Empty;
        public string Owner { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
