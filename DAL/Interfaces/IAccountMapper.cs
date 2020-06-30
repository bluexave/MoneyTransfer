using DAL.DataContext;
using DAL.Entities;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface IAccountMapper
    {
        public Task<Account> GetAccount(int account);
        public Task<Account> UpdateBalance(Account account, double amount);
        public Task<Account> AddAccount(string userName);
        public void SetContext(DatabaseContext context);
    }
}
