namespace Web.DTOs.Accounts
{
    public class CreateAccountRequest
    {
        public string Owner { get; set; } = string.Empty;
        public decimal InitialBalance { get; set; }
    }
}
