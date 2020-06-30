namespace MoneyTransfer.Contracts
{
    public class MoneyTransferRequest
    {
        public int Source { get; set; }
        public int Destination { get; set; }
        public double Amount { get; set; }
    }
}
