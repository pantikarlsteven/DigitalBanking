using DigitalBanking.Application.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Domain.Entities;

namespace DigitalBanking.Application.Interfaces
{
    public interface IAccountRepository
    {
        Task<ServiceResult<AccountDTO?>> GetAccountTransactionAsync(string accountNumber);
        Task<ServiceResult<decimal>> GetAccountBalanceAsync(string accountNumber);
        Task<ServiceResult<bool>> UpdateAccountStatusAsync(string accountNumber);
        Task<ServiceResult<AccountDTO>> AddAccountAsync(AddAccountDTO account);
    }
}
