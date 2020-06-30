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
                if (resAccount.Balance != initialBalance)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectAccountBalance);
                }
                if (resAccount.UserName != userName)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectUsername);
                }
                var outTran = await TransactionMapper.AddMTTransaction(resAccount, initialBalance);
                if (outTran == null || outTran.TransactionId == 0)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectTransactionId);
                }
                if (outTran.DestinationAccountId.AccountId != genaccountId)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectDestination);
                }
                if (outTran.SourceAccountId != null)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectSource);
                }
                if (outTran.TransferAmount != initialBalance)
                {
                    throw new BLException(KnownError.DALError_ReturnsIncorrectTransactionAmount);
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
    }
}
