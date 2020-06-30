using DAL.DataContext;
using DAL.Entities;
using System.Threading.Tasks;

namespace DAL.Interfaces
{
    public interface ITransactMapper
    {
        public Task<MTTransaction> AddMTTransaction(Account destinationAccount, double amount);
        public Task<MTTransaction> AddMTTransaction(Account destinationAccount, Account sourceAccount, double amount);
        public void SetContext(DatabaseContext context);
    }
}
