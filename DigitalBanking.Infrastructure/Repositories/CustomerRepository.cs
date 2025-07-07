using DigitalBanking.Application.Common;
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

        public async Task<ServiceResult<CustomerDTO>> FindAsync(Guid id)
        {
            var data = await _context.Customers
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

            if (data == null)
                return ServiceResult<CustomerDTO>.Fail($"Customer with id {id} not found");

            return ServiceResult<CustomerDTO>.Ok(data, "Customer retrieved successfully");
        }

        public async Task<ServiceResult<Guid>> AddAsync(AddCustomerDTO customer)
        {
            if (string.IsNullOrEmpty(customer.FirstName))
                return ServiceResult<Guid>.Fail("First Name is required");

            if (string.IsNullOrEmpty(customer.LastName))
                return ServiceResult<Guid>.Fail("Last Name is required");

            if (string.IsNullOrEmpty(customer.Email))
                return ServiceResult<Guid>.Fail("Email is required");

            if (string.IsNullOrEmpty(customer.Phone))
                return ServiceResult<Guid>.Fail("Phone is required");

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

            return ServiceResult<Guid>.Ok(newCustomer.Id, "Customer created successfully");
        }

        public async Task<ServiceResult<bool>> PutAsync(Guid id, UpdateCustomerInfo customer)
        {
            var customerExists = await _context.Customers.FindAsync(id);

            if (customerExists == null)
                return ServiceResult<bool>.Fail($"Customer with ID {id} was not found.");

            if (string.IsNullOrEmpty(customer.FirstName))
                return ServiceResult<bool>.Fail("First Name is required");

            if (string.IsNullOrEmpty(customer.LastName))
                return ServiceResult<bool>.Fail("Last Name is required");

            if (string.IsNullOrEmpty(customer.Email))
                return ServiceResult<bool>.Fail("Email is required");

            if (string.IsNullOrEmpty(customer.Phone))
                return ServiceResult<bool>.Fail("Phone is required");

            customerExists.FirstName = customer.FirstName;
            customerExists.LastName = customer.LastName;
            customerExists.Email = customer.Email;
            customerExists.Phone = customer.Phone;

            await _context.SaveChangesAsync();
            return ServiceResult<bool>.Ok(true, "Customer detail updated successfully");
        }
    }
}
