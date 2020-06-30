using BL.DALInterfaces;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace BL
{
    public class TransactionHandler : ITransactionHandler
    {
        private IDbContextTransaction _tran;
        private DatabaseContext _context;
        public void StartWork(IAccountMapper accountMapper, ITransactMapper transactMapper)
        {
            _context = new DatabaseContext(DatabaseContext.ops.dbOptions);
            _tran = _context.Database.BeginTransaction();
            accountMapper.SetContext(_context);
            transactMapper.SetContext(_context);
        }
        public void EndWork()
        {
            _tran.CommitAsync();
            CleanUp();
        }
        public void RollbackWork()
        {
            _tran.RollbackAsync();
            CleanUp();
        }
        private void CleanUp()
        {
            _context.Dispose();
        }
    }
}
