using Microsoft.AspNetCore.Mvc;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Core.Interfaces;
using System.Linq.Expressions;
using Web.Models;
using Core.Exceptions;
using Web.Models.Requests;

namespace Web.Controllers;

[ApiController]
[Route("[controller]")]
public class BankAccountController : ControllerBase
{
    private readonly IBankAccountRepository _bankAccountRepository;

    public BankAccountController(IBankAccountRepository bankAccountRepository)
    {
        _bankAccountRepository = bankAccountRepository;
    }

    [HttpPost("create")]
    public ActionResult<BankAccount> CreateBankAccount([FromBody] CreateBankAccountRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            throw new AppValidationException("Owner name is required.");

        BankAccount newAccount;
        
        switch (request.AccountType)
        {
            case AccountType.Credit:
                if (request.CreditLimit == null)
                    return BadRequest("Credit limit is required for a Line of Credit account.");
                newAccount = new LineOfCreditAccount(request.Name, request.InitialBalance, request.CreditLimit.Value);
                break;
            case AccountType.Gift:
                newAccount = new GiftCardAccount(request.Name, request.InitialBalance, request.MonthlyDeposit ?? 0);
                break;
            case AccountType.Interest:
                newAccount = new InterestEarningAccount(request.Name, request.InitialBalance);
                break;
            default:
                return BadRequest("Invalid account type.");
        }

        _bankAccountRepository.Add(newAccount);
        return CreatedAtAction(nameof(GetAccountInfo), new { accountNumber = newAccount.Number }, newAccount);
    }

    [HttpPost("monthEnd")]
    public ActionResult<string> PerformMonthEndForAccount([FromQuery] string accountNumber)
    {
        var account = _bankAccountRepository.GetByAccountNumber(accountNumber)
            ?? throw new AppValidationException("Cuenta no encontrada.");

        account.PerformMonthEndTransactions();
        return Ok($"Month-end processing completed for account {account.Number}.");
    }

    [HttpPost("deposit")]
    public ActionResult<string> MakeDeposit([FromQuery] decimal amount, [FromQuery] string note, [FromQuery] string accountNumber)
    {
        var account = _bankAccountRepository.GetByAccountNumber(accountNumber)
            ?? throw new AppValidationException("Cuenta no encontrada.");

        account.MakeDeposit(amount, DateTime.Now, note);
        _bankAccountRepository.Update(account);

        return Ok($"A deposit of ${amount} was made in account {account.Number}.");
    }

    [HttpPost("withdrawal")]
    public ActionResult<string> MakeWithdrawal([FromQuery] decimal amount, [FromQuery] string note, [FromQuery] string accountNumber)
    {
        var account = _bankAccountRepository.GetByAccountNumber(accountNumber)
            ?? throw new AppValidationException("Cuenta no encontrada.");

        account.MakeWithdrawal(amount, DateTime.Now, note);
        _bankAccountRepository.Update(account);


        return Ok($"A withdrawal of ${amount} was made in account {account.Number}.");
    }

    [HttpGet("balance")]
    public ActionResult<string> GetBalance([FromQuery] string accountNumber)
    {
        var account = _bankAccountRepository.GetByAccountNumber(accountNumber)
            ?? throw new AppValidationException("Cuenta no encontrada.");

        return Ok($"The balance in account {account.Number} is ${account.Balance}.");

    }

    [HttpGet("accountHistory")]
    public IActionResult GetAccountHistory([FromQuery] string accountNumber)
    {
        var account = _bankAccountRepository.GetByAccountNumber(accountNumber)
            ?? throw new AppValidationException("Cuenta no encontrada.");

        var history = account.GetAccountHistory();

        return Ok(history);
    }

    [HttpGet("accountInfo")]
    public ActionResult<BankAccountDto> GetAccountInfo([FromQuery] string accountNumber)
    {
        var account = _bankAccountRepository.GetByAccountNumber(accountNumber)
            ?? throw new AppValidationException("Cuenta no encontrada.");

        return BankAccountDto.Create(account);
    }

    [HttpGet("allAccountsInfo")]
    public ActionResult<List<BankAccountDto>> GetAllAccountInfo()
    {
        var list = _bankAccountRepository.ListWithTransaction();
        return BankAccountDto.Create(list);
    }
}