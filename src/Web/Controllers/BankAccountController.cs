using Core.Entities;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BankAccountController : ControllerBase
    {
        [HttpPost]
        public ActionResult<BankAccount> CreateAccount([FromQuery] string owner, [FromQuery] decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(owner))
                return BadRequest("El propietario es obligatorio.");

            if (initialBalance <= 0)
                return BadRequest("El saldo inicial debe ser mayor a 0.");

            var account = new BankAccount(owner, initialBalance);

            return Ok(account);
        }
    }
}
