namespace Core.Entities;

public class BankAccount
{
    // Campo estático: se comparte entre todas las instancias de BankAccount
    private static int s_accountNumberSeed = 1234567890;
    private List<Transaction> _allTransactions = new List<Transaction>();

    private readonly decimal _minimumBalance;

    // Campo de instancia: cada cuenta tiene su propio saldo
    public string Number { get; }
    public string Owner { get; set; }
    public AccountType AccountType { get; set; }
    public decimal Balance
    {
        get
        {
            decimal balance = 0;
            foreach (var item in _allTransactions)
            {
                balance += item.Amount;
            }

            return balance;
        }
    }

    public BankAccount(string name, decimal initialBalance): this(name, initialBalance, 0, AccountType.Gift) { }

    public BankAccount(string name, decimal initialBalance, decimal minimumBalance, AccountType accountType)
    {
            Number = s_accountNumberSeed.ToString();
            s_accountNumberSeed++;

            Owner = name;
            _minimumBalance = minimumBalance;
            AccountType = accountType; 

            if (initialBalance > 0)
                MakeDeposit(initialBalance, DateTime.Now, "Initial balance");
    }

    public void MakeDeposit(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount of deposit must be positive");
        }
        var deposit = new Transaction(amount, date, note);
        _allTransactions.Add(deposit);
    }

    public void MakeWithdrawal(decimal amount, DateTime date, string note)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "Amount of withdrawal must be positive");
        }
        Transaction? overdraftTransaction = CheckWithdrawalLimit(Balance - amount < _minimumBalance);
        Transaction? withdrawal = new(-amount, date, note);
        _allTransactions.Add(withdrawal);
        if (overdraftTransaction != null)
            _allTransactions.Add(overdraftTransaction);
    }

    protected virtual Transaction? CheckWithdrawalLimit(bool isOverdrawn)
    {
        if (isOverdrawn)
        {
            throw new InvalidOperationException("Not sufficient funds for this withdrawal");
        }
        else
        {
            return default;
        }
    }


    public string GetAccountHistory()
    {
        // Represents a mutable string of characters.
        var report = new System.Text.StringBuilder();

        decimal balance = 0;
        report.AppendLine("Date\t\tAmount\tBalance\tNote\t\tAccount Type");
        foreach (var item in _allTransactions)
        {
            balance += item.Amount;
            report.AppendLine($"{item.Date.ToShortDateString()}\t{item.Amount}\t{balance}\t{item.Notes}\t{this.AccountType}");
        }

        return report.ToString();
    }

    public virtual void PerformMonthEndTransactions() { }
}