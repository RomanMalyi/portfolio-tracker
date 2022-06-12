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

        [HttpGet]
        [ProducesResponseType(typeof(PolygonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok((await marketService.Get()).Skip(skip).Take(take));
        }

        [HttpGet("{ticker}")]
        [ProducesResponseType(typeof(PolygonResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string ticker)
        {
            return Ok((await marketService.Get()).FirstOrDefault(e => e.Ticker.Contains(ticker)));
        }

        [HttpGet("currencies")]
        [ProducesResponseType(typeof(List<CurrencyRate>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCurrencies()
        {
            return Ok(await marketService.GetCurrencies());
        }

        [HttpGet("tickers")]
        [ProducesResponseType(typeof(List<ShortResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromQuery] string[] tickers)
        {
            var market = await marketService.Get();
            var result = new List<ShortResponse>(tickers.Length);
            foreach (var ticker in tickers)
            {
                if (ticker != null)
                {
                    var marketValue = market.FirstOrDefault(e => e.Ticker.Contains(ticker));
                    if(marketValue!=null)
                    {
                        result.Add(new ShortResponse()
                        {
                            Ticker = ticker,
                            ClosePrice = marketValue.ClosePrice,
                            Change = (marketValue.ClosePrice - marketValue.OpenPrice)/marketValue.OpenPrice * 100
                        });
                    }
                }
            }

            return Ok(result);
        }
    }
}
