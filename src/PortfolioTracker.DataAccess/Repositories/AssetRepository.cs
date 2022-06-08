using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events.Common;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSharpFunctionalExtensions;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class AssetRepository
    {
        private static readonly IList<AssetAR> Assets;

        static AssetRepository()
        {
            Assets = new List<AssetAR>();
            AssetAR testAsset = new();
            testAsset.Create("testAsset", "test", "testUser", "Bitcoin", AssetType.Cryptocurrency, "BTC", 40000, Currency.USD, 0, 1230, RiskLevel.High);

            Assets.Add(testAsset);
        }

        public Maybe<AssetAR> GetById(string assetId)
        {
            AssetAR? assetAr = Assets.FirstOrDefault(a => a.IsAssetEqual(assetId));

            return assetAr ?? Maybe<AssetAR>.None;
        }

        public Task<PageResult<Asset>> Get(string accountId, int skip, int take)
        {
            int totalCount = Assets.Count(a => a.IsAccountEqual(accountId));
            List<AssetAR> result = Assets.Where(a => a.IsAccountEqual(accountId))
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

        public async Task Insert(AssetAR assetAr)
        {
            Assets.Add(assetAr);
        }

        public async Task Update(AssetAR assetAr)
        {
            //TODO: add logic, now it will be updated be reference
        }
    }
}
