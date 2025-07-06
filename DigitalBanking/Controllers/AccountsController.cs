using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DigitalBanking.Domain.Entities;
using DigitalBanking.Application.Interfaces;
using DigitalBanking.Application.DTOs;

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

        [HttpGet("{id}/getaccount")]
        public async Task<ActionResult<Account>> GetAccount(Guid id)
        {
            var account = await _repository.FindAsync(id);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        // GET: api/Accounts/{accountNumber}
        [HttpGet("{accountNumber}")]
        public async Task<ActionResult<Account>> GetAccountTransaction(string accountNumber)
        {
            var account = await _repository.GetAccountTransactionAsync(accountNumber);

            if (account == null)
            {
                return NotFound();
            }

            return account;
        }

        [HttpGet("{accountNumber}/balance")]
        public async Task<ActionResult<decimal>> GetAccountBalance(string accountNumber)
        {
            return await _repository.GetAccountBalanceAsync(accountNumber);
        }

        // PUT: api/Accounts/{accountNumber}/status
        [HttpPut("{accountNumber}/status")]
        public async Task<ActionResult<bool>> ActivateDeactivateAccount(string accountNumber)
        {
            return await _repository.ActivateDeactivateAccountAsync(accountNumber);
        }

        //// POST: api/Accounts
        [HttpPost]
        public async Task<ActionResult> PostAccount(AccountDTO account)
        {
            var id = await _repository.AddAsync(account);
            return CreatedAtAction("GetAccount", new { id }, account);
        }
    }
}

