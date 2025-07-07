using DigitalBanking.Application.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Domain.Constants;
using DigitalBanking.Domain.Entities;
using DigitalBanking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace DigitalBanking.Infrastructure.Repositories
{
    public class TransactionRepository: ITransactionRepository
    {
        private readonly AppDbContext _context;

        public TransactionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Transaction?> FindAsync(Guid id)
        {
            return await _context.Transactions.FindAsync(id);
        }

        public async Task<ServiceResult<List<Transaction>>> GetTransactionHistoryAsync(string accountNumber)
        {
            var accountExists = await _context.Accounts
                .FirstOrDefaultAsync(f => f.AccountNumber == accountNumber);

            if (accountExists == null)
                return ServiceResult<List<Transaction>>.Fail("Account not found");

            var transactions = await _context.Transactions
                .Where(w => w.FromAccountId == accountExists.Id)
                .OrderByDescending(o => o.Timestamp)
                .ToListAsync();

            return ServiceResult<List<Transaction>>.Ok(transactions, "Transaction history retrieved successfully");
        }


        public async Task<ServiceResult<Guid>> DepositAsync(DepositAmountDTO transaction)
        {
            if (transaction.Amount <= 0)
                return ServiceResult<Guid>.Fail("Deposit amount must be greater than zero.");

            var currentAccount = await _context.Accounts
                .FirstOrDefaultAsync(f => f.Id == transaction.FromAccountId);

            if (currentAccount == null)
                return ServiceResult<Guid>.Fail("Account not found.");

            var IsValid = await CheckDailyTransactionLimit(transaction.FromAccountId);

            if (!IsValid)
                return ServiceResult<Guid>.Fail($"Transaction Daily limit exceeds to {BusinessRules.DAILY_TRANSACTION_LIMIT}.");

            var newTrans = new Transaction
            {
                Id = Guid.NewGuid(),
                FromAccountId = transaction.FromAccountId,
                TransactionType = TransactionTypes.DEPOSIT,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                currentAccount.Balance += transaction.Amount;
                newTrans.Status = TransactionStatus.COMPLETED;

                _context.Transactions.Add(newTrans);
                await _context.SaveChangesAsync();

                return ServiceResult<Guid>.Ok(newTrans.Id, "Deposit successful.");
            }
            catch (Exception ex)
            {
                newTrans.Status = TransactionStatus.FAILED;

                try
                {
                    _context.Transactions.Add(newTrans);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                }

                return ServiceResult<Guid>.Fail("An error occurred during deposit. Transaction recorded as failed.");
            }
        }


        public async Task<ServiceResult<Guid>> WithdrawAsync(WithdrawAmountDTO transaction)
        {
            var currentAccount = await _context.Accounts
                .FirstOrDefaultAsync(f => f.Id == transaction.FromAccountId);

            if (currentAccount == null)
                return ServiceResult<Guid>.Fail("Account not found.");

            if (transaction.Amount <= 0)
                return ServiceResult<Guid>.Fail("Withdraw amount must be greater than 0.");

            var IsValid = await CheckDailyTransactionLimit(transaction.FromAccountId);

            if (!IsValid)
                return ServiceResult<Guid>.Fail($"Transaction Daily limit exceeds to {BusinessRules.DAILY_TRANSACTION_LIMIT}.");

            var newTrans = new Transaction
            {
                Id = Guid.NewGuid(),
                FromAccountId = transaction.FromAccountId,
                TransactionType = TransactionTypes.WITHDRAW,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                if (currentAccount.Balance < transaction.Amount)
                {
                    newTrans.Status = TransactionStatus.FAILED;
                }
                else
                {
                    currentAccount.Balance -= transaction.Amount;
                    newTrans.Status = TransactionStatus.COMPLETED;
                }

                _context.Transactions.Add(newTrans);
                await _context.SaveChangesAsync();

                if (newTrans.Status == TransactionStatus.FAILED)
                {
                    return ServiceResult<Guid>.Fail("Withdrawal failed: insufficient balance. Transaction recorded.");
                }

                return ServiceResult<Guid>.Ok(newTrans.Id, "Withdrawal successful.");
            }
            catch (Exception ex)
            {
                newTrans.Status = TransactionStatus.FAILED;

                try
                {
                    _context.Transactions.Add(newTrans);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                }

                return ServiceResult<Guid>.Fail("An error occurred during withdrawal. Transaction recorded as failed.");
            }
        }

        public async Task<ServiceResult<Guid>> TransferAsync(TransferAmountDTO transaction)
        {
            if (transaction.ToAccountId == null)
                return ServiceResult<Guid>.Fail("To Account not found");

            var fromAccount = await _context.Accounts
                .FirstOrDefaultAsync(f => f.Id == transaction.FromAccountId);

            var toAccount = await _context.Accounts
                .FirstOrDefaultAsync(f => f.Id == transaction.ToAccountId);

            if (fromAccount == null)
                return ServiceResult<Guid>.Fail("From Account not found.");

            if (toAccount == null)
                return ServiceResult<Guid>.Fail("To Account not found.");

            if (!fromAccount.IsActive)
                return ServiceResult<Guid>.Fail("From Account is inactive.");

            if (!toAccount.IsActive)
                return ServiceResult<Guid>.Fail("To Account is inactive.");

            if (transaction.Amount <= 0)
                return ServiceResult<Guid>.Fail("Transfer amount must be greater than 0.");

            var IsValid = await CheckDailyTransactionLimit(transaction.FromAccountId);

            if (!IsValid)
                return ServiceResult<Guid>.Fail($"Transaction Daily limit exceeds to {BusinessRules.DAILY_TRANSACTION_LIMIT}.");

            var newTrans = new Transaction
            {
                Id = Guid.NewGuid(),
                FromAccountId = transaction.FromAccountId,
                ToAccountId = transaction.ToAccountId,
                TransactionType = TransactionTypes.TRANSFER,
                Amount = transaction.Amount,
                Description = transaction.Description,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                transaction.Amount += BusinessRules.TRANSACTION_FEE;
                if (fromAccount.Balance < transaction.Amount)
                {
                    newTrans.Status = TransactionStatus.FAILED;
                }
                else
                {
                    fromAccount.Balance -= transaction.Amount;
                    toAccount.Balance += transaction.Amount;
                    newTrans.Status = TransactionStatus.COMPLETED;
                }

                _context.Transactions.Add(newTrans);
                await _context.SaveChangesAsync();

                if (newTrans.Status == TransactionStatus.FAILED)
                {
                    return ServiceResult<Guid>.Fail("Transfer failed: insufficient funds. Transaction recorded.");
                }

                return ServiceResult<Guid>.Ok(newTrans.Id, "Transfer successful.");
            }
            catch (Exception ex)
            {
                newTrans.Status = TransactionStatus.FAILED;

                try
                {
                    _context.Transactions.Add(newTrans);
                    await _context.SaveChangesAsync();
                }
                catch
                {
                }

                return ServiceResult<Guid>.Fail("An error occurred during transfer. Transaction recorded as failed.");
            }
        }


        public async Task<ServiceResult<List<Transaction>>> SearchByAmountOrDescriptionAsync(string searchInput)
        {
            decimal? searchAmount = null;

            if (decimal.TryParse(searchInput, out var amount))
            {
                searchAmount = amount;
            }

            searchInput = searchInput.Trim();

            var query = _context.Transactions.AsQueryable();

            query = query.Where(w =>
             (searchAmount != null && w.Amount == searchAmount) ||
             EF.Functions.Like(w.Description, $"%{searchInput}%"));


            var result = await query.ToListAsync();

            return ServiceResult<List<Transaction>>.Ok(result, "Search result retrieved successfully");
        }

        public async Task<ServiceResult<List<Transaction>>> GetAccountStatementAsync(string accountNumber, DateTime fromDate, DateTime toDate)
        {
            var account = await _context.Accounts.FirstOrDefaultAsync(f => f.AccountNumber == accountNumber);

            if (account == null)
                return ServiceResult<List<Transaction>>.Fail("Account not found");

            var transaction = await _context.Transactions
                .Where(w => w.FromAccountId == account.Id && w.Timestamp >= fromDate && w.Timestamp <= toDate)
                .ToListAsync();

            return ServiceResult<List<Transaction>>.Ok(transaction, "Account statement retrieved successfully");
        }

        private async Task<bool> CheckDailyTransactionLimit(Guid fromAccountId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            var totalAmount = await _context.Transactions
                .Where(w => w.FromAccountId == fromAccountId && w.Status == TransactionStatus.COMPLETED && w.Timestamp >= today && w.Timestamp < tomorrow)
                .SumAsync(s => s.Amount);

            return totalAmount <= BusinessRules.DAILY_TRANSACTION_LIMIT;
        }
    }
}