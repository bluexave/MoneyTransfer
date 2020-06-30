using DAL.DataContext;
using DAL.Entities;
using DAL.Interfaces;
using System.Threading.Tasks;

namespace DAL.DBFunctions
{
    public class AccountDBFunctions : IAccountMapper
    {
        private Account _account = new Account();
        private DatabaseContext _context;      
        public async Task<Account> AddAccount(string userName)
        {
            _account.UserName = userName;
            _context.Accounts.Add(_account);
            await _context.SaveChangesAsync();
            return _account;
        }
        public async Task<Account> UpdateBalance(Account account, double amount)
        {
            _account = account;
            _account.Balance = amount;
            _context.Accounts.Update(_account);
            await _context.SaveChangesAsync();
            return _account;
        }
        public async Task<Account> GetAccount(int account)
        {
            _account.AccountId = account;
            var outaccount = await _context.Accounts.FindAsync(_account.AccountId);
            return outaccount;
        }
        public void SetContext(DatabaseContext context)
        {
            _context = context;
        }
    }
}
