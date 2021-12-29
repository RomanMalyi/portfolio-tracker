using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.DataAccess;
using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.Api.Controllers
{
    //TODO: investigate OData
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserRepository userRepository;

        public UsersController(UserRepository userRepository)
        {
            this.userRepository = userRepository;
        }

        [HttpGet]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok(await userRepository.Get(skip, take));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await userRepository.Get(id));
        }

        [HttpPost]
        [ProducesResponseType(typeof(User), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromBody] string name)
        {
            return Ok(await userRepository.Create(name));
        }
    }
}
