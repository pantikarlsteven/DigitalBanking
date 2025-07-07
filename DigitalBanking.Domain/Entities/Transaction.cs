
namespace DigitalBanking.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}
