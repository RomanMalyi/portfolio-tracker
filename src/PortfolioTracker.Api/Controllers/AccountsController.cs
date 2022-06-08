using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Api.Dto.Account;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountsController : ControllerBase
    {
        private readonly AccountRepository accountRepository;

        public AccountsController(AccountRepository accountRepository)
        {
            this.accountRepository = accountRepository;
        }

        /// <summary>
        /// Get all accounts for user by id
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PageResult<Account>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            //TODO: get from claims
            string userId = "testUser";
            return Ok(await accountRepository.Get(userId, skip, take));
        }

        /// <summary>
        /// Get account by id
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string id)
        {
            //TODO: get userId from claims and validate if we need such method at all
            return Ok(await accountRepository.Get(id));
        }

        /// <summary>
        /// Creates new account
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] CreateAccount account)
        {
            //TODO: get from claims
            string userId = "testUser";
            return Ok(await accountRepository.Create(userId, account.Name, account.AccountType));
        }
    }
}
