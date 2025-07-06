using DigitalBanking.Application.DTOs;
using DigitalBanking.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DigitalBanking.Application.Interfaces
{
    public interface ICustomerRepository
    {
        Task<CustomerDTO?> FindAsync(Guid id);
        Task AddAsync(CustomerDTO customer);
        Task<bool> PutAsync(Guid id, CustomerDTO customer);
    }
}
