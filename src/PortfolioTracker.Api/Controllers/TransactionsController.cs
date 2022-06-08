using CSharpFunctionalExtensions;
using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Api.Dto.Transaction;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Domain;
using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.Api.Controllers
{
    [Route("api/assets/{assetId}/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly TransactionRepository transactionRepository;
        private readonly AssetRepository assetRepository;

        public TransactionsController(TransactionRepository transactionRepository,
            AssetRepository assetRepository)
        {
            this.transactionRepository = transactionRepository;
            this.assetRepository = assetRepository;
        }

        /// <summary>
        /// Get all transactions for asset by id
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PageResult<Transaction>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string assetId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok(await transactionRepository.Get(assetId, skip, take));
        }

        /// <summary>
        /// Creates new transaction
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Asset), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromRoute] string assetId, [FromBody] AddTransaction transaction)
        {
            //TODO: get from claims
            string userId = "testUser";

            Maybe<AssetAR> assetAggregateRoot = assetRepository.GetById(assetId);
            if (assetAggregateRoot.HasNoValue)
                return BadRequest($"Asset with the id{assetId} not found");


            var creationResult = assetAggregateRoot.Value.AddTransaction(assetId, userId, transaction.Amount, transaction.Description, transaction.TransactionType,
                transaction.TransactionDate, transaction.FromAssetId, transaction.ToAssetId, transaction.ExchangeRate);
            await assetRepository.Update(assetAggregateRoot.Value);
            transactionRepository.AddTransaction(creationResult.Value);

            return Ok(assetAggregateRoot.Value.Get().Value);
        }
    }
}
