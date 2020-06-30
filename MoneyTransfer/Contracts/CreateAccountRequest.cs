namespace MoneyTransfer.Contracts
{
    public class CreateAccountRequest
    {
        public string Username { get; set; }
        public double InitialBalance { get; set; }
    }
}
