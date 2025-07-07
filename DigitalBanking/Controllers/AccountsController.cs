using DigitalBanking.Api.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBanking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountRepository _repository;
        public AccountsController(IAccountRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Accounts/{accountNumber}
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<ApiResponse>> GetAccountTransaction(string accountNumber)
        {
            var result = await _repository.GetAccountTransactionAsync(accountNumber);

            if (!result.Success)
                return BadRequest(ApiResponse.FailResult(result.Message ?? "Account not found"));

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        [HttpGet("{accountNumber}/balance")]
        public async Task<ActionResult<ApiResponse>> GetAccountBalance(string accountNumber)
        {
            var result = await _repository.GetAccountBalanceAsync(accountNumber);

            if (!result.Success)
                return BadRequest(ApiResponse.FailResult(result.Message ?? "Account not found"));

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        // PUT: api/Accounts/{accountNumber}/status
        [HttpPut("{accountNumber}/status")]
        public async Task<ActionResult<ApiResponse>> UpdateAccountStatus(string accountNumber)
        {
            var result = await _repository.UpdateAccountStatusAsync(accountNumber);

            if (!result.Success)
                return BadRequest(ApiResponse.FailResult(result.Message ?? "Account not found"));

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        // POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult<ApiResponse>> PostAccount(AddAccountDTO account)
        {
            try
            {
                var result = await _repository.AddAccountAsync(account);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse.FailResult(result.Message ?? "Create Account failed"));
                }

                return Ok(ApiResponse.SuccessResult(result.Data, result.Message));

            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ApiResponse.FailResult(ex.Message));
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponse.FailResult("An unexpected error occurred."));
            }

        }
    }
}

