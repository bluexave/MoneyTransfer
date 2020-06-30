using System;

namespace BL
{
    public enum KnownError
    {
        InvalidInitialBalance,
        InvalidUsername,
        UsernameExists,
        DALError_ReturnsIncorrectAccountId,
        DALError_ReturnsIncorrectAccountBalance,
        DALError_ReturnsIncorrectUsername,
        DALError_ReturnsIncorrectTransactionId,
        DALError_ReturnsIncorrectDestination,
        DALError_ReturnsIncorrectSource,
        DALError_ReturnsIncorrectTransactionAmount
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
                case KnownError.DALError_ReturnsIncorrectAccountId:
                    blmessage = "DB error. Unexpected result for AccountId. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectAccountBalance:
                    blmessage = "DB error. Unexpected result for Account Balance. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectUsername:
                    blmessage = "DB error. Unexpected result for Account Username. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectTransactionId:
                    blmessage = "DB error. Unexpected result for TransactionId. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectDestination:
                    blmessage = "DB error. Unexpected result for DestinationId. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectSource:
                    blmessage = "DB error. Unexpected result for SourceId. Please contact your System Admin.";
                    break;
                case KnownError.DALError_ReturnsIncorrectTransactionAmount:
                    blmessage = "DB error. Unexpected result for TransactionAmount. Please contact your System Admin.";
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
