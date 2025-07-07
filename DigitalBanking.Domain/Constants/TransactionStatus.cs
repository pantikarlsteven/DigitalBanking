using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBanking.Domain.Constants
{
    public static class TransactionStatus
    {
        public const string PENDING = "Pending";
        public const string COMPLETED = "Completed";
        public const string FAILED = "Failed";
    }
}