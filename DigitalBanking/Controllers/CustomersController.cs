using DigitalBanking.Api.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

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
            var result = await _repository.FindAsync(id);

            if (!result.Success)
            {
                return BadRequest(ApiResponse.FailResult(result.Message ?? "Customer not found"));
            }

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        // PUT: api/Customers/{id}
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResponse>> PutCustomer(Guid id, UpdateCustomerInfo customer)
        {
            if (id != customer.Id)
            {
                return BadRequest(ApiResponse.FailResult("ID in route does not match ID in body"));
            }

            var result = await _repository.PutAsync(id, customer);

            if (!result.Success)
            {
                return NotFound(ApiResponse.FailResult(result.Message ?? "Customer not found"));
            }

            return Ok(ApiResponse.SuccessResult(null, result.Message));
        }

        // POST: api/Customers
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> PostCustomer(AddCustomerDTO customer)
        {
            var result = await _repository.AddAsync(customer);

            if (!result.Success)
                return ApiResponse.FailResult(result.Message ?? "Create Failed");

            return CreatedAtAction("GetCustomer", new { id = result.Data },
                   ApiResponse.SuccessResult(customer, result.Message ?? "Create successful"));

        }
    }
}
