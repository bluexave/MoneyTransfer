using System.ComponentModel.DataAnnotations;

namespace DAL.Entities
{
    public class Account
    {
        [Key]
        public int AccountId { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required]
        public double Balance { get; set; }
    }
}
