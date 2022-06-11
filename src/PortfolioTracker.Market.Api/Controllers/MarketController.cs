using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Market.Api.Dto;
using PortfolioTracker.Market.Api.Infrastructure;

namespace PortfolioTracker.Market.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MarketController : ControllerBase
    {
        private readonly MarketService marketService;

        public MarketController(MarketService marketService)
        {
            this.marketService = marketService;
        }

        /// <summary>
        /// Get all accounts for user by id
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PolygonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok(marketService.Get().Skip(skip).Take(take));
        }

        /// <summary>
        /// Get all accounts for user by id
        /// </summary>
        [HttpGet("{ticker}")]
        [ProducesResponseType(typeof(PolygonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string ticker)
        {
            return Ok(marketService.Get().FirstOrDefault(e=>e.Ticker.Contains(ticker)));
        }

        /// <summary>
        /// Get all accounts for user by id
        /// </summary>
        [HttpGet("currencies")]
        [ProducesResponseType(typeof(CurrencyRate), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrencies()
        {
            return Ok(marketService.GetCurrencies());
        }
    }
}
