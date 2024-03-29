﻿using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Api.Dto.Asset;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.DataAccess.Repositories;
using PortfolioTracker.Domain;
using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.Api.Controllers
{
    [Route("api/accounts/{accountId}/[controller]")]
    [ApiController]
    public class AssetsController : ControllerBase
    {
        private readonly AssetArRepository assetArRepository;
        private readonly AssetRepository assetRepository;

        public AssetsController(AssetArRepository assetArRepository, AssetRepository assetRepository)
        {
            this.assetArRepository = assetArRepository;
            this.assetRepository = assetRepository;
        }

        /// <summary>
        /// Get all account assets
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PageResult<Asset>), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get([FromRoute] string accountId, [FromQuery] int skip = 0, [FromQuery] int take = 10)
        {
            return Ok(await assetRepository.GetByAccount(accountId, skip, take));
        }

        /// <summary>
        /// Creates new asset
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(Asset), StatusCodes.Status200OK)]
        public async Task<IActionResult> Post([FromRoute] string accountId, [FromBody] CreateAsset asset)
        {
            //TODO: get from claims
            string userId = "testUser";

            AssetAR assetAggregateRoot = new();
            assetAggregateRoot.Create(accountId, userId, asset.Name, asset.AssetType, asset.ExchangeTicker,
                asset.OpenPrice, asset.Currency, asset.InterestRate, asset.Units, asset.RiskLevel);

            await assetArRepository.Upsert(assetAggregateRoot);

            return Ok(assetAggregateRoot.Get().Value);
        }
    }
}
