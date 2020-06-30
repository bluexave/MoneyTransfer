using BL.DALInterfaces;
using System;
using System.Threading.Tasks;

namespace BL
{
    public class Manager
    {
        public IAccountMapper AccountMapper { get; }
        public ITransactMapper TransactionMapper { get; }
        public ITransactionHandler TransactionHandler { get; }

        public Manager(IAccountMapper accountMapper, ITransactMapper transactMapper, ITransactionHandler transactionHandler)
        {
            AccountMapper = accountMapper;
            TransactionMapper = transactMapper;
            TransactionHandler = transactionHandler;
        }

        public async Task<int?> CreateNewUser(string userName, double initialBalance)
        {
            TransactionHandler.StartWork(this.AccountMapper, this.TransactionMapper);
            try
            {
                int genaccountId;
                if (initialBalance < 0)
                {
                    throw new BLException(KnownError.InvalidInitialBalance);
                }
                if (String.IsNullOrEmpty(userName))
                {
                    throw new BLException(KnownError.InvalidUsername);
                }
                var resAccount = await AccountMapper.AddAccount(userName);
                if (resAccount == null || resAccount.AccountId == 0)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectAccountId);
                }
                genaccountId = resAccount.AccountId;
                resAccount = await AccountMapper.UpdateBalance(resAccount, initialBalance);
                if (resAccount == null || resAccount.AccountId != genaccountId)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectAccountId);
                }                
                var outTran = await TransactionMapper.AddMTTransaction(resAccount, initialBalance);
                if (outTran == null || outTran.TransactionId == 0)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectTransactionId);
                }                                
                TransactionHandler.EndWork();
                return resAccount.AccountId;
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    if (ex.InnerException.Message.Contains("IX_Accounts_UserName"))
                    {
                        ex = new BLException(KnownError.UsernameExists);
                    }
                }
                TransactionHandler.RollbackWork();
                throw ex;
            }
        }
        public async Task<int?> Transfer(int source, int destination, double amount)
        {
            TransactionHandler.StartWork(this.AccountMapper, this.TransactionMapper);
            try
            {
                if (amount < 0)
                {
                    throw new BLException(KnownError.InvalidTransferAmount);
                }
                var sourceAccount = await AccountMapper.GetAccount(source);
                var destinationAccount = await AccountMapper.GetAccount(destination);
                if (sourceAccount == null || sourceAccount.AccountId == 0)
                {
                    throw new BLException(KnownError.InvalidSource);
                }
                if (destinationAccount == null || destinationAccount.AccountId == 0)
                {
                    throw new BLException(KnownError.InvalidDestination);
                }
                if (sourceAccount.Balance < amount)
                {
                    throw new BLException(KnownError.InvalidTransferAmount);
                }
                //_context.Entry(sourceAccount).State = EntityState.Detached;
                //_context.Entry(destinationAccount).State = EntityState.Detached;
                sourceAccount = await AccountMapper.UpdateBalance(sourceAccount, sourceAccount.Balance - amount);
                destinationAccount = await AccountMapper.UpdateBalance(destinationAccount, destinationAccount.Balance + amount);
                var outTran = await TransactionMapper.AddMTTransaction(destinationAccount, sourceAccount, amount);
                TransactionHandler.EndWork();
                return outTran.TransactionId;
            }
            catch (Exception ex)
            {
                TransactionHandler.RollbackWork();
                throw ex;
            }
        }
    }
}
