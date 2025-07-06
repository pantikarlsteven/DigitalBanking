
namespace DigitalBanking.Domain.Entities
{
    public class Account
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; } 
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public Customer Customer { get; set; }
    }
}
