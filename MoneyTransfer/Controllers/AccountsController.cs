using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BL;
using MoneyTransfer.Contracts;

namespace MoneyTransfer.Controllers
{
    [Route("api/Account")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private Manager manager;
        public AccountsController()
        {
            manager = new Manager(new DAL.DBFunctions.AccountDBFunctions(), new DAL.DBFunctions.MTTransactionDBFunctions(), new BL.TransactionHandler());
        }

        [Route("add")]
        [HttpPost]
        public async Task<ActionResult<CreateAccountResult>> CreateAccount(CreateAccountRequest account)
        {
            var res = await manager.CreateNewUser(account.Username, account.InitialBalance);
            return new CreateAccountResult
            {
                AccountId = res.Value
            };
        }
    }
}
