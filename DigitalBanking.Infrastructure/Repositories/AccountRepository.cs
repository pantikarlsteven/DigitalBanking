using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Domain.Entities;
using DigitalBanking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DigitalBanking.Infrastructure.Repositories
{
    public class AccountRepository: IAccountRepository
    {
        private readonly AppDbContext _context;
        public AccountRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Account?> FindAsync(Guid id)
        {
            return await _context.Accounts
                    .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Account> GetAccountTransactionAsync(string accountNumber)
        {
            return null;
        }

        public async Task<decimal> GetAccountBalanceAsync(string accountNumber)
        {
            var account = await AccountExists(accountNumber);

            if (account == null)
            {
                throw new KeyNotFoundException($"Account number {accountNumber} was not found.");
            }

            return account.Balance;
        }

        public async Task<bool> ActivateDeactivateAccountAsync(string accountNumber)
        {
            var account = await AccountExists(accountNumber);
            if (account == null)
            {
                throw new KeyNotFoundException($"Account number {accountNumber} was not found.");
            }

            account.IsActive = !account.IsActive;
            
            await _context.SaveChangesAsync();

            return account.IsActive;
        }

        public async Task<Guid> AddAsync(AccountDTO account)
        {
            var customerExists = await _context.Customers.FindAsync(account.CustomerId);

            if (customerExists == null)
            {
                throw new KeyNotFoundException($"Customer with id {account.CustomerId} was not found.");
            }

            var newAccount = new Account
            {
                Id = Guid.NewGuid(),
                AccountNumber = account.AccountNumber,
                CustomerId = account.CustomerId,
                AccountType = account.AccountType,
                Balance = account.Balance,
                IsActive = account.IsActive,
                CreatedDate = DateTime.UtcNow
            };

            _context.Accounts.Add(newAccount);
            await _context.SaveChangesAsync();

            return newAccount.Id;
        }

        public async Task<Account?> AccountExists(string accountNumber)
        {
            var accountExists = await _context.Accounts.FirstOrDefaultAsync(w => w.AccountNumber == accountNumber);
            return accountExists;
        }
    }
}
