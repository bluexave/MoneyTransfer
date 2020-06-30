using DAL.DataContext;
using DAL.Entities;
using System.Threading.Tasks;

namespace BL.DALInterfaces
{
    public interface ITransactMapper
    {
        public Task<MTTransaction> AddMTTransaction(Account destinationAccount, double amount);
        public Task<MTTransaction> AddMTTransaction(Account destinationAccount, Account sourceAccount, double amount);
        public void SetContext(DatabaseContext context);
    }
}
