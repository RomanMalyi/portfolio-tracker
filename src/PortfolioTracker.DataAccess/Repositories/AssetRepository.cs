using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class AssetRepository
    {
        private static readonly IList<AssetAR> Assets;

        static AssetRepository()
        {
            Assets = new List<AssetAR>();
            AssetAR testAsset = new();
            testAsset.Create("testAsset", "test", "testUser", "Bitcoin", Currency.USD, 10, 1230, RiskLevel.High);

            Assets.Add(testAsset);
        }

        public Task<PageResult<Asset>> Get(string accountId, int skip, int take)
        {
            int totalCount = Assets.Count(a => a.IsEqual(accountId));
            List<AssetAR> result = Assets.Where(a => a.IsEqual(accountId))
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(
                new PageResult<Asset>()
                {
                    Data = result.Select(e => e.Get().Value).ToList(),
                    Skip = skip,
                    Take = take,
                    TotalCount = totalCount
                });
        }

        public async Task Upsert(AssetAR assetAr)
        {
            Assets.Add(assetAr);
        }
    }
}
