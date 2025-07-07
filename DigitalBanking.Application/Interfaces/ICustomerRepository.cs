using DigitalBanking.Application.Common;
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
        Task<ServiceResult<CustomerDTO>> FindAsync(Guid id);
        Task<ServiceResult<Guid>> AddAsync(AddCustomerDTO customer);
        Task<ServiceResult<bool>> PutAsync(Guid id, UpdateCustomerInfo customer);
    }
}
