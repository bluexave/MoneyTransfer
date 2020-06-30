using BL.DALInterfaces;
using DAL.DataContext;
using DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Security.Cryptography.X509Certificates;

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
        public void Detach(object o)
        {
            _context.Entry(o).State = EntityState.Detached;
        }
    }
}
