namespace DigitalBanking.Application.DTOs
{
    public class TransactionDTO
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

    public class TransferAmountDTO
    {
        public Guid FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public class WithdrawAmountDTO
    {
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }

    public class DepositAmountDTO
    {
        public Guid FromAccountId { get; set; }
        public decimal Amount { get; set; }
        public string? Description { get; set; }
    }
}
