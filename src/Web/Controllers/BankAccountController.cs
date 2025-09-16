using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Infrastructure.Data;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class BankAccountController : ControllerBase
{
    
    private readonly ApplicationDbContext _context;

    public BankAccountController(ApplicationDbContext applicationDbContext)
    {
        _context = applicationDbContext;
    }

    [HttpPost("create")]
    public ActionResult<BankAccount> CreateBankAccount([FromQuery] string name, [FromQuery] decimal initialBalance, [FromQuery] AccountType accountType, [FromQuery] decimal? creditLimit = null, [FromQuery] decimal? monthlyDeposit = null)  
    {
        try
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Owner name is required.");

            BankAccount newAccount;

            switch (accountType)
            {
                case AccountType.Credit:
                    if (creditLimit == null)
                        return BadRequest("Credit limit is required for a Line of Credit account.");
                    newAccount = new LineOfCreditAccount(name, initialBalance, creditLimit.Value);
                    break;

                case AccountType.Gift:
                    newAccount = new GiftCardAccount(name, initialBalance, monthlyDeposit ?? 0);
                    break;

                case AccountType.Interest:
                    newAccount = new InterestEarningAccount(name, initialBalance);
                    break;

                default:
                    return BadRequest("Invalid account type.");
            }

            // accounts.Add(newAccount);
            _context.bankAccounts.Add(newAccount);
            _context.SaveChanges();

        return CreatedAtAction(nameof(GetAccountInfo), new { accountNumber = newAccount.Number }, newAccount);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost("monthEnd")]
    public ActionResult<string> PerformMonthEndForAccount([FromQuery] string accountNumber)
    {
        try
        {
            var account = _context.bankAccounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null)
                return NotFound("Account not found.");

            account.PerformMonthEndTransactions();
            _context.SaveChanges();
            return Ok($"Month-end processing completed for account {account.Number}.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }


    [HttpPost("deposit")]
    public ActionResult<string> MakeDeposit([FromQuery] decimal amount, [FromQuery] string note, [FromQuery] string accountNumber)
    {
        try
        {
            var account = _context.bankAccounts.FirstOrDefault(a => a.Number == accountNumber);

            if (account == null)
                return NotFound("Cuenta no encontrada.");

            account.MakeDeposit(amount, DateTime.Now, note);
            _context.SaveChanges();

            return Ok($"A deposit of ${amount} was made in account {account.Number}.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpPost("withdrawal")]
    public ActionResult<string> MakeWithdrawal([FromQuery] decimal amount, [FromQuery] string note, [FromQuery] string accountNumber)
    {
        try
        {
            var account = _context.bankAccounts.FirstOrDefault(a => a.Number == accountNumber);

            if (account == null)
                return NotFound("Cuenta no encontrada.");

            account.MakeWithdrawal(amount, DateTime.Now, note);
            _context.SaveChanges();

            return Ok($"A withdrawal of ${amount} was made in account {account.Number}.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("balance")]
    public ActionResult<string> GetBalance([FromQuery] string accountNumber)
    {
        try
        {
            var account = _context.bankAccounts.FirstOrDefault(a => a.Number == accountNumber);

            if (account == null)
                return NotFound("Cuenta no encontrada.");

            return Ok($"The balance in account {account.Number} is ${account.Balance}.");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("accountHistory")]
    public IActionResult GetAccountHistory([FromQuery] string accountNumber)
    {
        try
        {
            var account = _context.bankAccounts.FirstOrDefault(a => a.Number == accountNumber);

            if (account == null)
                return NotFound("Cuenta no encontrada.");

            var history = account.GetAccountHistory();

            return Ok(history);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("accountInfo")]
    public IActionResult GetAccountInfo([FromQuery] string accountNumber)
    {
        try
        {
            var account = _context.bankAccounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null)
                return NotFound("Cuenta no encontrada.");

            var accountInfo = new
            {
                account.Number,
                account.Owner,
                Balance = account.Balance
            };

            return Ok(accountInfo);
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }

    [HttpGet("allAccountsInfo")]
    public IActionResult GetAllAccountInfo()
    {
        try
        {
            var allInfo = _context.bankAccounts.Select(account => new 
            {
                account.Number,
                account.Owner,
                Balance = account.Balance
            });

            return Ok(allInfo);
        }
        catch (Exception ex)
        {

            return StatusCode(500, $"Error interno del servidor: {ex.Message}");
        }
    }
}