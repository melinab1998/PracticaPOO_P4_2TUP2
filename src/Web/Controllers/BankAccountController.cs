using Core.Entities;
using Microsoft.AspNetCore.Mvc;
using Web.DTOs.Accounts;
using Web.DTOs.Transactions;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class BankAccountController : ControllerBase
    {
        public static List<BankAccount> Accounts = new List<BankAccount>();

        [HttpPost]
        public ActionResult<BankAccountResponse> CreateAccount([FromBody] CreateAccountRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Owner))
                return BadRequest(new { message = "El propietario es obligatorio." });

            if (request.InitialBalance <= 0)
                return BadRequest(new { message = "El saldo inicial debe ser mayor a 0." });

            var account = new BankAccount(request.Owner, request.InitialBalance);
            Accounts.Add(account);

            var response = new BankAccountResponse
            {
                Number = account.Number,
                Owner = account.Owner,
                Balance = account.Balance
            };

            return CreatedAtAction(nameof(GetBalance), new { accountNumber = account.Number }, response);
        }

        [HttpPost("{accountNumber}/deposit")]
        public ActionResult<TransactionResponse> Deposit(string accountNumber, [FromBody] TransactionRequest request)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null)
                return NotFound(new { message = $"Cuenta {accountNumber} no encontrada." });

            try
            {
                account.MakeDeposit(request.Amount, DateTime.Now, request.Note);
                var response = new TransactionResponse
                {
                    Amount = request.Amount,
                    Note = request.Note,
                    Date = DateTime.Now
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("{accountNumber}/withdraw")]
        public ActionResult<TransactionResponse> Withdraw(string accountNumber, [FromBody] TransactionRequest request)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null)
                return NotFound(new { message = $"Cuenta {accountNumber} no encontrada." });

            try
            {
                account.MakeWithdrawal(request.Amount, DateTime.Now, request.Note);
                var response = new TransactionResponse
                {
                    Amount = request.Amount, 
                    Note = request.Note,
                    Date = DateTime.Now
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }


        [HttpGet("{accountNumber}/balance")]
        public ActionResult<BalanceResponse> GetBalance(string accountNumber)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null)
                return NotFound(new { message = $"Cuenta {accountNumber} no encontrada." });

            var response = new BalanceResponse
            {
                Owner = account.Owner,
                Balance = account.Balance
            };

            return Ok(response);
        }

        [HttpGet("{accountNumber}/history")]
        public ActionResult<string> GetHistory(string accountNumber)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null)
                return NotFound($"Cuenta {accountNumber} no encontrada.");

            return Ok(account.GetAccountHistory());
        }

    }
}
