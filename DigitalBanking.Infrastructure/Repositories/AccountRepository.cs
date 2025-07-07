using DigitalBanking.Application.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Domain.Entities;
using DigitalBanking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.Infrastructure.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<ServiceResult<AccountDTO?>> GetAccountTransactionAsync(string accountNumber)
        {
            var result = await _context.Accounts
                .Where(w =>  w.AccountNumber == accountNumber)
                .Select(s => new AccountDTO
                {
                    Id = s.Id,
                    AccountNumber = s.AccountNumber,
                    AccountType = s.AccountType,
                    CustomerId = s.CustomerId,
                    Balance = s.Balance,
                    Transactions = _context.Transactions
                        .Where(w => w.FromAccountId == s.Id)
                        .OrderByDescending(o => o.Timestamp)
                        .Select(s => new TransactionDTO
                        {
                            Id = s.Id,
                            FromAccountId = s.FromAccountId,
                            ToAccountId = s.ToAccountId,
                            TransactionType = s.TransactionType,
                            Amount = s.Amount,
                            Description = s.Description,
                            Status = s.Status,
                            Timestamp = s.Timestamp
                        }).ToList()
                })
                .FirstOrDefaultAsync();

            return ServiceResult<AccountDTO?>.Ok(result, "Account Transactions retrieved successfully");
        }

        public async Task<ServiceResult<decimal>> GetAccountBalanceAsync(string accountNumber)
        {
            var current = await AccountExists(accountNumber);

            if (current == null)
                return ServiceResult<decimal>.Fail("Account not found");

            return ServiceResult<decimal>.Ok(current.Balance, "Account balance retrieved successfully");
        }

        public async Task<ServiceResult<bool>> UpdateAccountStatusAsync(string accountNumber)
        {
            var current = await AccountExists(accountNumber);
           
            if (current == null)
                return ServiceResult<bool>.Fail("Account not found");

            current.IsActive = !current.IsActive;
            
            await _context.SaveChangesAsync();

            return ServiceResult<bool>.Ok(current.IsActive, "Account status updated successfully");
        }

        public async Task<ServiceResult<AccountDTO>> AddAccountAsync(AddAccountDTO account)
        {
            var customerExists = await _context.Customers.FindAsync(account.CustomerId);

            if (customerExists == null)
                return ServiceResult<AccountDTO>.Fail($"Customer with id {account.CustomerId} was not found.");

            if (account.Balance <= 0)
                return ServiceResult<AccountDTO>.Fail("Balance must be greater than 0");

            string generatedAccountNumber = await GenerateUniqueAccountNumberAsync();

            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = generatedAccountNumber,
                CustomerId = account.CustomerId,
                AccountType = account.AccountType,
                Balance = account.Balance,
                IsActive = true,
                CreatedDate = DateTime.UtcNow
            };

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                _context.Accounts.Add(newAccount);
                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();

                var addedAccount = new AccountDTO
                {
                    Id = newAccount.Id,
                    AccountNumber = newAccount.AccountNumber,
                    CustomerId = newAccount.CustomerId,
                    AccountType = newAccount.AccountType,
                    Balance = newAccount.Balance,
                    IsActive = newAccount.IsActive,
                };

                return ServiceResult<AccountDTO>.Ok(addedAccount, "Withdrawal successful.");
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return ServiceResult<AccountDTO>.Fail("An error occurred during withdrawal. Transaction rolled back.");
            }
        }

        private async Task<string> GenerateUniqueAccountNumberAsync()
        {
            var random = new Random();
            string accountNumber;
            bool exists;

            do
            {
                int length = random.Next(8, 11); // 8, 9 or 10
                accountNumber = string.Concat(Enumerable.Range(0, length).Select(_ => random.Next(0, 10).ToString()));

                exists = await _context.Accounts.AnyAsync(a => a.AccountNumber == accountNumber);

            } while (exists);

            return accountNumber;
        }

        private async Task<Account?> AccountExists(string accountNumber)
        {
            var accountExists = await _context.Accounts.FirstOrDefaultAsync(w => w.AccountNumber == accountNumber);
            return accountExists;
        }
    }
}
