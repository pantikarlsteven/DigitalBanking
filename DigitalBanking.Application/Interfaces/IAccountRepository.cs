using DigitalBanking.Application.DTOs;
using DigitalBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBanking.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<Account?> FindAsync(Guid id);
        Task<Account> GetAccountTransactionAsync(string accountNumber);
        Task<decimal> GetAccountBalanceAsync(string accountNumber);
        Task<bool> ActivateDeactivateAccountAsync(string accountNumber);
        Task<Guid> AddAsync(AccountDTO account);
    }
}
