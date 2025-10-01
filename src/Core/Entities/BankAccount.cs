using Core.Exceptions;

namespace Core.Entities;

public class BankAccount
{
    public int Id { get; set; }

    private List<Transaction> _allTransactions = new List<Transaction>();
    public IReadOnlyCollection<Transaction> Transactions => _allTransactions;

    private readonly decimal _minimumBalance;

    private readonly decimal _withDrawalLimit = 100000;

    // Campo de instancia: cada cuenta tiene su propio saldo
    public string Number { get; set; }
    public string Owner { get; set; }

    protected BankAccount() // protected: visibilidad a las clases hijas
    {

    }

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

    public BankAccount(string name, decimal initialBalance) : this(name, initialBalance, 0) { }

    public BankAccount(string name, decimal initialBalance, decimal minimumBalance)
    {
        Number = Guid.NewGuid().ToString();

        Owner = name;
        _minimumBalance = minimumBalance;
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

        if (Balance - amount < 0)
        {
            throw new AppValidationException("No hay saldo suficiente en la cuenta.");
        }

        if (amount > _withDrawalLimit)
        {
            throw new AppValidationException(
            $"El monto {amount} excede el límite de extracción.");
        }

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
            string accountType = this.GetType().Name;
            report.AppendLine($"{item.Date.ToShortDateString()}\t{item.Amount}\t{balance}\t{item.Notes}\t{accountType}");
        }

        return report.ToString();
    }

    public virtual void PerformMonthEndTransactions() { }
}