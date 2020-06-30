using System.Threading.Tasks;
using BL;
using Microsoft.AspNetCore.Mvc;
using MoneyTransfer.Contracts;

namespace MoneyTransfer.Controllers
{
    [Route("api/Transfer")]
    [ApiController]
    public class MoneyTransferController : ControllerBase
    {
        private Manager manager;
        public MoneyTransferController()
        {
            manager = new Manager(new DAL.DBFunctions.AccountDBFunctions(), new DAL.DBFunctions.MTTransactionDBFunctions(), new BL.TransactionHandler());
        }

        [Route("initiate")]
        [HttpPost]
        public async Task<ActionResult<MoneyTransferResult>> Transfer(MoneyTransferRequest req)
        {
            var res = await manager.Transfer(req.Source, req.Destination, req.Amount);
            return new MoneyTransferResult
            {
                TransactionId = res.Value
            };

        }
    }
}
