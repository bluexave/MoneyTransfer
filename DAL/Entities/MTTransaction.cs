using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class MTTransaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public Account DestinationAccountId { get; set; }
        public Account SourceAccountId { get; set; }
        [Required]
        public double TransferAmount { get; set; }
    }
}