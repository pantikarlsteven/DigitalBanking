using DigitalBanking.Api.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DigitalBanking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly ICustomerRepository _repository;

        public CustomersController(ICustomerRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Customers/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetCustomer(Guid id)
        {
            var customer = await _repository.FindAsync(id);

            if (customer == null)
            {
                return NotFound(ApiResponse.FailResult($"Customer with ID {id} not found"));
            }

            return Ok(ApiResponse.SuccessResult(customer, "Customer retrieved successfully"));
        }

        // PUT: api/Customers/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> PutCustomer(Guid id, CustomerDTO customer)
        {
            if (id != customer.Id)
            {
                return BadRequest(ApiResponse.FailResult("ID in route does not match ID in body"));
            }

            var result = await _repository.PutAsync(id, customer);

            if (!result)
            {
                return NotFound(ApiResponse.FailResult($"Customer with ID {id} not found"));
            }

            return Ok(ApiResponse.SuccessResult(null, "Customer updated successfully"));
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> PostCustomer(CustomerDTO customer)
        {
            await _repository.AddAsync(customer);

            var response = ApiResponse.SuccessResult(customer, "Customer created successfully");

            return CreatedAtAction("GetCustomer", new { id = customer.Id }, response);
        }
    }
}
