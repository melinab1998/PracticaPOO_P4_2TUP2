using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TransactionController : ControllerBase
    {
        [HttpPost("deposit")]
        public ActionResult<string> Deposit([FromQuery] string accountNumber, [FromQuery] decimal amount)
        {
            var account = BankAccountController.Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            try
            {
                account.MakeDeposit(amount, DateTime.Now, "Depósito");
                return Ok($"Depósito realizado: {amount}. Saldo actual: {account.Balance}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("withdraw")]
        public ActionResult<string> Withdraw([FromQuery] string accountNumber, [FromQuery] decimal amount)
        {
            var account = BankAccountController.Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            try
            {
                account.MakeWithdrawal(amount, DateTime.Now, "Retiro");
                return Ok($"Retiro realizado: {amount}. Saldo actual: {account.Balance}");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("balance")]
        public ActionResult<string> GetBalance([FromQuery] string accountNumber)
        {
            var account = BankAccountController.Accounts.FirstOrDefault(a => a.Number == accountNumber);
            if (account == null) return NotFound($"Cuenta {accountNumber} no encontrada.");

            return Ok($"El saldo actual de la cuenta {account.Number} es: {account.Balance}");
        }
    }
}
