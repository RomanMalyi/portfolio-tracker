using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class AssetRepository
    {
        private const string AssetTableName = "dbo.Asset";

        private readonly SqlDatabase sqlDatabase;

        public AssetRepository(SqlDatabase sqlDatabase)
        {
            this.sqlDatabase = sqlDatabase;
        }

        public async Task<PageResult<Asset>> GetByAccount(string accountId, int skip, int take)
        {
            int totalCount = await sqlDatabase.ExecuteScalar<int>(@$"Select count(*) from {AssetTableName} where AccountId = '{accountId}'", CancellationToken.None);

            var query = @$"Select * from {AssetTableName} where AccountId = '{accountId}'
                            ORDER BY Id
                            OFFSET {skip} ROWS
                            FETCH NEXT {take} ROWS ONLY;";

            var result = await sqlDatabase.Query<Asset>(query, CancellationToken.None);

            return
                new PageResult<Asset>()
                {
                    Data = result.ToList(),
                    Skip = skip,
                    Take = take,
                    TotalCount = totalCount
                };
        }


        public async Task<PageResult<Asset>> GetByUser(string userId, int skip, int take)
        {
            int totalCount = await sqlDatabase.ExecuteScalar<int>(@$"Select count(*) from {AssetTableName} where UserId = '{userId}'", CancellationToken.None);

            var query = @$"Select * from {AssetTableName} where UserId = '{userId}'
                            ORDER BY Id
                            OFFSET {skip} ROWS
                            FETCH NEXT {take} ROWS ONLY;";

            var result = await sqlDatabase.Query<Asset>(query, CancellationToken.None);

            return
                new PageResult<Asset>()
                {
                    Data = result.ToList(),
                    Skip = skip,
                    Take = take,
                    TotalCount = totalCount
                };
        }

        public Task Create(AssetCreated asset)
        {
            DateTimeOffset createdAt = DateTimeOffset.UtcNow;

            string exchangeTickerColumn = asset.ExchangeTicker != null ? "[ExchangeTicker]," : string.Empty;
            string exchangeTickerValue = asset.ExchangeTicker != null ? $"'{asset.ExchangeTicker}'," : string.Empty;
            string openPriceColumn = asset.OpenPrice != null ? "[OpenPrice]," : string.Empty;
            string openPriceValue = asset.OpenPrice != null ? $"{asset.OpenPrice}," : string.Empty;
            string interestRateColumn = asset.InterestRate != null ? "[InterestRate]," : string.Empty;
            string interestRateValue = asset.InterestRate != null ? $"{asset.InterestRate}," : string.Empty;

            var command = @$"Insert into {AssetTableName}
                ([Id], [AccountId], [UserId], [Name], [AssetType], {exchangeTickerColumn} {openPriceColumn} {interestRateColumn} [Units], [Currency], [RiskLevel], [CreatedAt] )
                    Values
                ('{asset.Id}', '{asset.AccountId}', '{asset.UserId}', '{asset.Name}', '{asset.AssetType}',
                {exchangeTickerValue}{openPriceValue}{interestRateValue}{asset.Units},'{asset.Currency}','{asset.RiskLevel}','{createdAt}')";

            return sqlDatabase.ExecuteNonQuery(command, CancellationToken.None);
        }

        public async Task Delete(string id)
        {
            var command = $"Delete from {AssetTableName} where [Id] = '{id}'";

            await sqlDatabase.ExecuteNonQuery(command, CancellationToken.None);
        }
    }
}
