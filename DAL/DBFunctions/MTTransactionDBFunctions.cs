using DAL.DataContext;
using DAL.Entities;
using DAL.Interfaces;
using System.Threading.Tasks;

namespace DAL.DBFunctions
{
    public class MTTransactionDBFunctions : ITransactMapper
    {
        private MTTransaction _mtTrans = new MTTransaction();
        private DatabaseContext _context;    
        public async Task<MTTransaction> AddMTTransaction(Account destinationAccount, double amount)
        {
            _mtTrans.DestinationAccountId = destinationAccount;
            _mtTrans.TransferAmount = amount;
            _context.MTTransactions.Add(_mtTrans);
            await _context.SaveChangesAsync();
            return _mtTrans;
        }
        public async Task<MTTransaction> AddMTTransaction(Account destinationAccount, Account sourceAccount, double amount)
        {
            _mtTrans.DestinationAccountId = destinationAccount;
            _mtTrans.SourceAccountId = sourceAccount;
            _mtTrans.TransferAmount = amount;
            _context.MTTransactions.Add(_mtTrans);
            await _context.SaveChangesAsync();
            return _mtTrans;
        }
        public void SetContext(DatabaseContext context)
        {
            _context = context;
        }
    }
}
