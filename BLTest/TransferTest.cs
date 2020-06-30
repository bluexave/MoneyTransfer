using BL;
using BL.DALInterfaces;
using DAL.Entities;
using DAL.Interfaces;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System.Threading.Tasks;

namespace BLTest
{
    [TestFixture]
    public class TransferTest
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
            Assert.ThrowsAsync<BLException>(async () => await mgr.Transfer(1,2,-100));
        }
        [Test]
        public void ThrowsBLExceptionWhenSourceDoesNotExist()
        {
            int inputAccountId = 1;
            Account resSourceAcct = null;
            moqAccount.Setup(x => x.GetAccount(inputAccountId)).Returns(Task.FromResult(resSourceAcct));
            Assert.ThrowsAsync<BLException>(async () => await mgr.Transfer(inputAccountId, 2, 100));
        }
        [Test]
        public void ThrowsBLExceptionWhenDestinationDoesNotExist()
        {
            int inputAccountId = 1;
            int inputDestinationId = 2;            
            Account resSourceAcct = new Account { AccountId = inputAccountId, UserName="testUser1", Balance = 1000};
            Account resDestAcct = null;
            moqAccount.Setup(x => x.GetAccount(inputAccountId)).Returns(Task.FromResult(resSourceAcct));
            moqAccount.Setup(x => x.GetAccount(inputDestinationId)).Returns(Task.FromResult(resDestAcct));
            Assert.ThrowsAsync<BLException>(async () => await mgr.Transfer(inputAccountId, inputDestinationId, 100));
        }
        [Test]
        public void ThrowsBLExceptionWhenSourceBalanceLessThanAmount()
        {
            int inputAccountId = 1;
            int inputDestinationId = 2;
            double inputTransferAmount = 100;
            double sourceBalance = 50;
            Account resSourceAcct = new Account { AccountId = inputAccountId, UserName = "testUser1", Balance = sourceBalance };
            Account resDestAcct = new Account { AccountId = inputDestinationId, UserName = "testUser2", Balance = 1000 };
            moqAccount.Setup(x => x.GetAccount(inputAccountId)).Returns(Task.FromResult(resSourceAcct));
            moqAccount.Setup(x => x.GetAccount(inputDestinationId)).Returns(Task.FromResult(resDestAcct));
            Assert.ThrowsAsync<BLException>(async () => await mgr.Transfer(inputAccountId, inputDestinationId, inputTransferAmount));
        }
        //Use Transaction Tests for Concurrency Scenario Support
        [Test]
        public void HappyPathCallsBeginsTransactionAndCommitAsync()
        {
            int inputAccountId = 1;
            int inputDestinationId = 2;
            int generatedTransactionId = 2;
            double inputTransferAmount = 100;
            double sourceBalance = 1000;
            double destinationBalance = 200;
            Account resSourceAcct = new Account { AccountId = inputAccountId, UserName = "testUser1", Balance = sourceBalance };
            Account resDestAcct = new Account { AccountId = inputDestinationId, UserName = "testUser2", Balance = destinationBalance };
            Account newresSourceAcct = new Account { AccountId = inputAccountId, UserName = "testUser1", Balance = sourceBalance - inputTransferAmount };
            Account newresDestAcct = new Account { AccountId = inputDestinationId, UserName = "testUser2", Balance = destinationBalance + inputTransferAmount};
            MTTransaction resultFromAddTransact = new MTTransaction
            {
                TransactionId = generatedTransactionId,
                DestinationAccountId = new Account { AccountId = inputDestinationId, Balance = inputTransferAmount, UserName = "testUser2" },
                SourceAccountId = new Account { AccountId = inputAccountId, Balance = inputTransferAmount, UserName = "testUser1" },
                TransferAmount = inputTransferAmount
            };
            moqAccount.Setup(x => x.GetAccount(inputAccountId)).Returns(Task.FromResult(resSourceAcct));
            moqAccount.Setup(x => x.GetAccount(inputDestinationId)).Returns(Task.FromResult(resDestAcct));
            moqAccount.Setup(x => x.UpdateBalance(resSourceAcct, sourceBalance - inputTransferAmount)).Returns(Task.FromResult(newresSourceAcct));
            moqAccount.Setup(x => x.UpdateBalance(resDestAcct, destinationBalance + inputTransferAmount)).Returns(Task.FromResult(newresDestAcct));
            moqTransact.Setup(x => x.AddMTTransaction(newresDestAcct, newresSourceAcct, inputTransferAmount)).Returns(Task.FromResult(resultFromAddTransact));
            var sut = mgr.Transfer(inputAccountId, inputDestinationId, inputTransferAmount);
            moqTranHandler.Verify(x => x.StartWork(moqAccount.Object, moqTransact.Object), Times.Once);
            //moqTranHandler.Verify(x => x.EndWork(), Times.Once);
            Assert.AreEqual(generatedTransactionId, sut.Result);
        }
        [Test]
        public void ErrorPathCallsBeginsTransactionAndRollbackAsync()
        {
            int inputAccountId = 1;
            int inputDestinationId = 2;
            double inputTransferAmount = 100;
            double sourceBalance = 50;
            Account resSourceAcct = new Account { AccountId = inputAccountId, UserName = "testUser1", Balance = sourceBalance };
            Account resDestAcct = new Account { AccountId = inputDestinationId, UserName = "testUser2", Balance = 1000 };
            moqAccount.Setup(x => x.GetAccount(inputAccountId)).Returns(Task.FromResult(resSourceAcct));
            moqAccount.Setup(x => x.GetAccount(inputDestinationId)).Returns(Task.FromResult(resDestAcct));
            Assert.ThrowsAsync<BLException>(async () => await mgr.Transfer(inputAccountId, inputDestinationId, inputTransferAmount));
            moqTranHandler.Verify(x => x.StartWork(moqAccount.Object, moqTransact.Object), Times.Once);
            moqTranHandler.Verify(x => x.RollbackWork(), Times.Once);
        }
    }
}
