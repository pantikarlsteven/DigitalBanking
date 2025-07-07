using DigitalBanking.Api.Common;
using DigitalBanking.Application.DTOs;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace DigitalBanking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly ITransactionRepository _repository;

        public TransactionsController(ITransactionRepository repository)
        {
            _repository = repository;
        }

        // GET: api/Transactions/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse>> GetTransaction(Guid id)
        {
            var transaction = await _repository.FindAsync(id);

            if (transaction == null)
            {
                return NotFound(ApiResponse.FailResult($"Transaction with id {id} not found"));
            }

            return Ok(ApiResponse.SuccessResult(transaction, "Transaction retrieved successfully"));
        }

        // GET: api/accounts/{accountNumber}/transactions
        [HttpGet("{accountNumber}/transactions")]
        public async Task<ActionResult<ApiResponse>> GetTransactionHistory(string accountNumber)
        {
            var result = await _repository.GetTransactionHistoryAsync(accountNumber);

            if (!result.Success)
                return NotFound(ApiResponse.FailResult(result.Message ?? "Account not found"));

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        [HttpGet("{searchInput}/searchByAmountOrDescription")]
        public async Task<ActionResult<ApiResponse>> SearchByAmountOrDescription(string searchInput)
        {
            if (string.IsNullOrWhiteSpace(searchInput))
                return BadRequest(ApiResponse.FailResult("Search input is required"));

            var result = await _repository.SearchByAmountOrDescriptionAsync(searchInput);

            if (!result.Success)
                return BadRequest(ApiResponse.FailResult(result.Message ?? "Search Failed"));

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        [HttpGet("{accountNumber}/accountStatement")]
        public async Task<ActionResult<ApiResponse>> GetAccountStatement(string accountNumber, DateTime fromDate, DateTime toDate)
        {
            var result = await _repository.GetAccountStatementAsync(accountNumber, fromDate, toDate);

            if (!result.Success)
                return BadRequest(ApiResponse.FailResult(result.Message ?? "Get Account statement failed"));

            return Ok(ApiResponse.SuccessResult(result.Data, result.Message));
        }

        // POST: api/Transactions/deposit
        [HttpPost("deposit")]
        public async Task<ActionResult<ApiResponse>> Deposit(DepositAmountDTO transaction)
        {
            try
            {
                var result = await _repository.DepositAsync(transaction);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse.FailResult(result.Message ?? "Deposit failed"));
                }

                return CreatedAtAction("GetTransaction", new { id = result.Data },
                    ApiResponse.SuccessResult(transaction, result.Message ?? "Deposit successful"));
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

        // POST: api/Transactions/withdraw
        [HttpPost("withdraw")]
        public async Task<ActionResult<ApiResponse>> Withdraw(WithdrawAmountDTO transaction)
        {
            try
            {
                var result = await _repository.WithdrawAsync(transaction);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse.FailResult(result.Message ?? "Withdrawal failed"));
                }

                return CreatedAtAction("GetTransaction", new { id = result.Data },
                    ApiResponse.SuccessResult(transaction, result.Message ?? "Withdrawal successful"));
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


        // POST: api/Transactions/transfer
        [HttpPost("transfer")]
        public async Task<ActionResult<ApiResponse>> Transfer(TransferAmountDTO transaction)
        {
            try
            {
                var result = await _repository.TransferAsync(transaction);

                if (!result.Success)
                {
                    return BadRequest(ApiResponse.FailResult(result.Message ?? "Transfer failed"));
                }

                return CreatedAtAction("GetTransaction", new { id = result.Data },
                    ApiResponse.SuccessResult(transaction, result.Message ?? "Transfer successful"));
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
