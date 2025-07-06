using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Domain.Entities;
using DigitalBanking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.Infrastructure.Repositories
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly AppDbContext _context;

        public CustomerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<CustomerDTO?> FindAsync(Guid id)
        {
            return await _context.Customers
               .Where(c => c.Id == id)
               .Select(c => new CustomerDTO
               {
                   Id = c.Id,
                   FirstName = c.FirstName,
                   LastName = c.LastName,
                   Email = c.Email,
                   Phone = c.Phone,
                   CreatedDate = c.CreatedDate,
                   Accounts = c.Accounts.Select(a => new AccountDTO
                   {
                       Id = a.Id,
                       AccountNumber = a.AccountNumber,
                       CustomerId = a.CustomerId,
                       AccountType = a.AccountType,
                       Balance = a.Balance,
                       IsActive = a.IsActive,
                   }).ToList()
               })
               .FirstOrDefaultAsync();
        }

        public async Task AddAsync(CustomerDTO customer)
        {
            var newCustomer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                Phone = customer.Phone,
                CreatedDate = DateTime.UtcNow
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> PutAsync(Guid id, CustomerDTO customer)
        {
            var customerExists = await _context.Customers.FindAsync(id);

            if (customerExists == null)
            {
                throw new KeyNotFoundException($"Customer with ID {id} was not found.");
            }

            customerExists.FirstName = customer.FirstName;
            customerExists.LastName = customer.LastName;
            customerExists.Email = customer.Email;
            customerExists.Phone = customer.Phone;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CustomerExists(id))
                {
                    return false;
                }
                else
                {
                    throw;
                }
            }

        }

        public bool CustomerExists(Guid id)
        {
            return _context.Customers.Any(e => e.Id == id);
        }
    }
}
