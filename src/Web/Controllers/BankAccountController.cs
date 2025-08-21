using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("bankaccount")]
    public class BankAccountController : ControllerBase
    {
        public static List<BankAccount> Accounts = new List<BankAccount>();

        [HttpPost("create")]
        public ActionResult<BankAccount> CreateAccount([FromQuery] string owner, [FromQuery] decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(owner))
                return BadRequest("El propietario es obligatorio.");

            if (initialBalance <= 0)
                return BadRequest("El saldo inicial debe ser mayor a 0.");

            var account = new BankAccount(owner, initialBalance);
            Accounts.Add(account);

            return Ok(account);
        }

        [HttpPost("{accountNumber}/deposit")]
        public ActionResult<string> Deposit([FromRoute] string accountNumber, [FromQuery] decimal amount, [FromQuery] string note = "Depósito")
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            try
            {
                account.MakeDeposit(amount, DateTime.Now, note);
                return Ok($"Depósito de {amount} realizado en {account.Number}. Saldo actual: {account.Balance}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("{accountNumber}/withdraw")]
        public ActionResult<string> Withdraw([FromRoute] string accountNumber, [FromQuery] decimal amount, [FromQuery] string note = "Retiro")
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            try
            {
                account.MakeWithdrawal(amount, DateTime.Now, note);
                return Ok($"Retiro de {amount} realizado en {account.Number}. Saldo actual: {account.Balance}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{accountNumber}/balance")]
        public ActionResult<string> GetBalance([FromRoute] string accountNumber)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            return Ok($"Saldo actual de {account.Owner} (Cuenta {account.Number}): {account.Balance}");
        }

        [HttpGet("{accountNumber}/history")]
        public ActionResult<string> GetHistory([FromRoute] string accountNumber)
        {
            var account = Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            return Ok(account.GetAccountHistory());
        }
    }
}
