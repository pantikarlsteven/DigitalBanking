using DigitalBanking.Application.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Domain.Entities;

namespace DigitalBanking.Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task<Transaction?> FindAsync(Guid id);
        Task<ServiceResult<List<Transaction>>> GetTransactionHistoryAsync(string accountNumber);
        Task<ServiceResult<Guid>> DepositAsync(DepositAmountDTO transaction);
        Task<ServiceResult<Guid>> WithdrawAsync(WithdrawAmountDTO transaction);
        Task<ServiceResult<Guid>> TransferAsync(TransferAmountDTO transaction);
        Task<ServiceResult<List<Transaction>>> SearchByAmountOrDescriptionAsync(string searchInput);
        Task<ServiceResult<List<Transaction>>> GetAccountStatementAsync(string accountNumber, DateTime fromDate, DateTime toDate);
    }
}
