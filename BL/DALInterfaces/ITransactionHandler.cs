using DAL.Interfaces;

namespace BL.DALInterfaces
{
    public interface ITransactionHandler
    {
        public void StartWork(IAccountMapper accountMapper, ITransactMapper transactMapper);
        public void EndWork();
        public void RollbackWork();
    }
}
