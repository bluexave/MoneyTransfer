using BL;
using BL.DALInterfaces;
using DAL.Entities;
using DAL.Interfaces;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Threading.Tasks;

namespace BLTest
{
    [TestFixture]
    public class CreateAccountTest
    {
        private Manager mgr;
        private Mock<IAccountMapper> moqAccount;
        private Mock<ITransactMapper> moqTransact;
        private Mock<ITransactionHandler> moqTranHandler;

        [SetUp]
        public void SetUp()
        {
            moqAccount = new Mock<IAccountMapper>();
            moqTransact = new Mock<ITransactMapper>();
            moqTranHandler = new Mock<ITransactionHandler>();
            mgr = new Manager(moqAccount.Object, moqTransact.Object, moqTranHandler.Object);
        }

        [Test]
        public void ThrowsBLExceptionWhenInitialBalanceIsNegative()
        {
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser("testUser", -1000));
        }
        [Test]
        public void ThrowsBLExceptionWhenUserNameIsEmptyString()
        {
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser("", 1000));
        }
        [Test]
        public void ThrowsBLExceptionWhenDALReturns0forAccountIdFromAddAccount()
        {
            string inputUser = "existingUser";
            double inputbalance = 1000;
            Account resultAccount = new Account
            {
                AccountId = 0
            };
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultAccount));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }
        [Test]
        public void ThrowsBLExceptionWhenDALReturnsNullAccountFromAddAccount()
        {
            string inputUser = "existingUser";
            double inputbalance = 1000;
            Account resultAccount = null;
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultAccount));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }
        [Test]
        public void ThrowsBLExceptionWhenDALReturnsNullAccountFromUpdateBalance()
        {
            string inputUser = "existingUser";
            double inputbalance = 1000;
            Account resultFromAddAccount = new Account
            {
                AccountId = 1,
                UserName = inputUser,
                Balance = 0
            };
            Account resultFromUpdate = null;
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultFromAddAccount));
            moqAccount.Setup(x => x.UpdateBalance(resultFromAddAccount, inputbalance)).Returns(Task.FromResult(resultFromUpdate));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }
        [Test]
        public void ThrowsBLExceptionWhenDALReturnsIncorrectAccountIdFromUpdateBalance()
        {
            string inputUser = "existingUser";
            double inputbalance = 1000;
            int generatedAccountId = 1;
            int updatedAccountId = 2;
            Account resultFromAddAccount = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = 0
            };
            Account resultFromUpdate = new Account
            {
                AccountId = updatedAccountId,
                UserName = inputUser,
                Balance = inputbalance
            };
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultFromAddAccount));
            moqAccount.Setup(x => x.UpdateBalance(resultFromAddAccount, inputbalance)).Returns(Task.FromResult(resultFromUpdate));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }        
        [Test]
        public void ThrowsBLExceptionWhenDALReturns0TransactionIdFromAddTransaction()
        {
            string inputUser = "existingUser";
            double inputbalance = 1000;
            int generatedAccountId = 1;
            int generatedTransactionId = 0;
            Account resultFromAddAccount = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = 0
            };
            Account resultFromUpdate = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = inputbalance
            };
            MTTransaction resultFromAddTransact = new MTTransaction
            {
                TransactionId = generatedTransactionId,
                DestinationAccountId = resultFromUpdate,
                TransferAmount = inputbalance,
                SourceAccountId = null
            };
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultFromAddAccount));
            moqAccount.Setup(x => x.UpdateBalance(resultFromAddAccount, inputbalance)).Returns(Task.FromResult(resultFromUpdate));
            moqTransact.Setup(x => x.AddMTTransaction(resultFromUpdate, inputbalance)).Returns(Task.FromResult(resultFromAddTransact));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }
        [Test]
        public void ThrowsBLExceptionWhenDALReturnsNullTransactionFromAddTransaction()
        {
            string inputUser = "existingUser";
            double inputbalance = 1000;
            int generatedAccountId = 1;
            Account resultFromAddAccount = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = 0
            };
            Account resultFromUpdate = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = inputbalance
            };
            MTTransaction resultFromAddTransact = null;
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultFromAddAccount));
            moqAccount.Setup(x => x.UpdateBalance(resultFromAddAccount, inputbalance)).Returns(Task.FromResult(resultFromUpdate));
            moqTransact.Setup(x => x.AddMTTransaction(resultFromUpdate, inputbalance)).Returns(Task.FromResult(resultFromAddTransact));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }       
        [Test]
        //Unique Constraint Violation for User Exists Scenario
        public void ThrowsBLExceptionWhenDALThrowsUniqueRecordExceptionForUsernameField()
        {
            string inputUser = "userExists";
            double inputbalance = 1000;
            Exception ex = new Exception("moqException", new Exception("IX_Accounts_UserName"));
            moqAccount.Setup(x => x.AddAccount(inputUser)).Throws(ex);
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
        }
        //Use Transaction Tests for Concurrency Scenario Support
        [Test]
        public void HappyPathCallsBeginsTransactionAndCommitAsync()
        {
            string inputUser = "goodUser";
            double inputbalance = 1000;
            int generatedAccountId = 1;
            int generatedTransactionId = 1;
            Account resultFromAddAccount = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = 0
            };
            Account resultFromUpdate = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = inputbalance
            };
            MTTransaction resultFromAddTransact = new MTTransaction
            {
                TransactionId = generatedTransactionId,
                DestinationAccountId = new Account { AccountId = generatedAccountId, Balance = inputbalance, UserName = inputUser },
                SourceAccountId = null,
                TransferAmount = inputbalance
            };
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultFromAddAccount));
            moqAccount.Setup(x => x.UpdateBalance(resultFromAddAccount, inputbalance)).Returns(Task.FromResult(resultFromUpdate));
            moqTransact.Setup(x => x.AddMTTransaction(resultFromUpdate, inputbalance)).Returns(Task.FromResult(resultFromAddTransact));
            var sut = mgr.CreateNewUser(inputUser, inputbalance);
            moqTranHandler.Verify(x => x.StartWork(moqAccount.Object, moqTransact.Object), Times.Once);
            moqTranHandler.Verify(x => x.EndWork(), Times.Once);
            Assert.AreEqual(generatedAccountId, sut.Result);        
        }
        [Test]
        public void ErrorPathCallsBeginsTransactionAndRollbackAsync()
        {
            string inputUser = "goodUser";
            double inputbalance = 1000;
            int generatedAccountId = 1;
            int generatedTransactionId = 0;           
            Account resultFromAddAccount = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = 0
            };
            Account resultFromUpdate = new Account
            {
                AccountId = generatedAccountId,
                UserName = inputUser,
                Balance = inputbalance
            };
            MTTransaction resultFromAddTransact = new MTTransaction
            {
                TransactionId = generatedTransactionId,
                DestinationAccountId = new Account { AccountId = generatedAccountId, Balance = inputbalance, UserName = inputUser },
                SourceAccountId = null,
                TransferAmount = inputbalance
            };
            moqAccount.Setup(x => x.AddAccount(inputUser)).Returns(Task.FromResult(resultFromAddAccount));
            moqAccount.Setup(x => x.UpdateBalance(resultFromAddAccount, inputbalance)).Returns(Task.FromResult(resultFromUpdate));
            moqTransact.Setup(x => x.AddMTTransaction(resultFromUpdate, inputbalance)).Returns(Task.FromResult(resultFromAddTransact));
            Assert.ThrowsAsync<BLException>(async () => await mgr.CreateNewUser(inputUser, inputbalance));
            moqTranHandler.Verify(x => x.StartWork(moqAccount.Object, moqTransact.Object), Times.Once);
            moqTranHandler.Verify(x => x.RollbackWork(), Times.Once);
        }
    }
}
