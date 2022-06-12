using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.Api.Dto;
using PortfolioTracker.DataAccess.Repositories;

namespace PortfolioTracker.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AnalyticsController : ControllerBase
    {
        private readonly SnapshotRepository snapshotRepository;

        public AnalyticsController(SnapshotRepository snapshotRepository)
        {
            this.snapshotRepository = snapshotRepository;
        }

        /// <summary>
        /// Get all accounts for user by id
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(AnalyticsResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Get()
        {
            //TODO: get from claims
            string userId = "testUser";

            var snapshots = await snapshotRepository.Get(userId);

            if (snapshots.Count < 2)
                return Ok(new AnalyticsResponse());

            var yesterdaySnapshot = snapshots[snapshots.Count - 2];
            var currentSnapshot = snapshots.LastOrDefault();

            var result = new AnalyticsResponse()
            {
                CurrentTotalAmount = currentSnapshot.TotalAmount,
                HighestLevelOfRisk = currentSnapshot.RiskLevelAnalytics.Max(e => e.RiskLevel).ToString(),
                MostPopularCurrency = currentSnapshot.CurrencyAnalytics
                    .First(e => e.PortfolioAmount
                        .Equals(currentSnapshot.CurrencyAnalytics.Max(e => e.PortfolioAmount)))
                    .Currency.ToString(),
                NumberOfAccounts = currentSnapshot.AccountAnalytics.Count,
                NumberOfAsstTypes = currentSnapshot.AssetTypeAnalytics.Count,
                Snapshots = snapshots,
                PortfolioChange = (yesterdaySnapshot.TotalAmount - currentSnapshot.TotalAmount) / yesterdaySnapshot.TotalAmount * 100,
            };

            return Ok(result);
        }
    }
}
