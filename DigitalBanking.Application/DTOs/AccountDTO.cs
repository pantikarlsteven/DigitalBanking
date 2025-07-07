using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBanking.Application.DTOs
{
    public class AccountDTO
    {
        public Guid Id { get; set; }
        public string AccountNumber { get; set; }
        public Guid CustomerId { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
        public bool IsActive { get; set; }
        public List<TransactionDTO>? Transactions { get; set; }

    }

    public class AddAccountDTO
    {
        public Guid CustomerId { get; set; }
        public string AccountType { get; set; }
        public decimal Balance { get; set; }
    }
}
