using System;

namespace BL
{
    public enum KnownError
    {
        InvalidInitialBalance,
        InvalidUsername,
        UsernameExists,
        InvalidTransferAmount,
        InvalidSource,
        InvalidDestination,
        DALError_ReturnsIncorrectAccountId,
        DALError_ReturnsIncorrectTransactionId       
    }
    public class BLException : Exception
    {
        private string blmessage;
        public BLException(KnownError blerrortype)
        {
            switch (blerrortype)
            {
                case KnownError.InvalidInitialBalance:
                    blmessage = "Failed to Create Account. Invalid Initial Balance.";
                    break;
                case KnownError.InvalidUsername:
                    blmessage = "Failed to Create Account. Invalid Username.";
                    break;
                case KnownError.UsernameExists:
                    blmessage = "Failed to Create Account. Username exists.";
                    break;
                case KnownError.InvalidTransferAmount:
                    blmessage = "Transfer Failed. Invalid Amount.";
                    break;
                case KnownError.InvalidSource:
                    blmessage = "Transfer Failed. Invalid Source Account.";
                    break;
                case KnownError.InvalidDestination:
                    blmessage = "Transfer Failed. Invalid Destination Account.";
                    break;
                case KnownError.DALError_ReturnsIncorrectAccountId:
                    blmessage = "DB error. Unexpected result for AccountId. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectTransactionId:
                    blmessage = "DB error. Unexpected result for TransactionId. Please contact your System Admin.";
                    break;                
            }
        }
        public override string Message
        {
            get
            {
                return blmessage;
            }
        }
    }
}
