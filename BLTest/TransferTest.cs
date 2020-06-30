using BL;
using BL.DALInterfaces;
using DAL.Entities;
using Moq;
using NUnit.Framework;
using NUnit.Framework.Internal;
using System;
using System.Collections.Generic;
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
    }
}
