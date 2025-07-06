using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBanking.Application.DTOs
{
    public class AccountTransactionDTO
    {
        public string AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public Guid FromAccountId { get; set; }
        public Guid? ToAccountId { get; set; }
        public decimal Amount { get; set; }
        public string TransactionType { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string Status { get; set; }
    }
}
