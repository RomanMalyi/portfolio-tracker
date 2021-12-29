using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.DataAccess;
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
        [HttpGet]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok(await accountRepository.Get(skip, take));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await accountRepository.Get(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(Account), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] Account account)
        {
            return Ok(await accountRepository.Create(account));
        }
    }
}
