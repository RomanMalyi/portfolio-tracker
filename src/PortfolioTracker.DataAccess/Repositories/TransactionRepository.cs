using System;
using System.Linq;
using System.Threading;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class TransactionRepository
    {
        private const string TransactionTableName = "[dbo].[Transaction]";

        private readonly SqlDatabase sqlDatabase;

        public TransactionRepository(SqlDatabase sqlDatabase)
        {
            this.sqlDatabase = sqlDatabase;
        }

        public async Task<PageResult<Transaction>> Get(string assetId, int skip, int take)
        {
            int totalCount = await sqlDatabase.ExecuteScalar<int>(@$"Select count(*) from {TransactionTableName} where AssetId = '{assetId}'", CancellationToken.None);

            var query = @$"Select * from {TransactionTableName} where AssetId = '{assetId}'
                            ORDER BY Id
                            OFFSET {skip} ROWS
                            FETCH NEXT {take} ROWS ONLY;";

            var result = await sqlDatabase.Query<Transaction>(query, CancellationToken.None);

            return 
                new PageResult<Transaction>()
                {
                    Data = result.ToList(),
                    Skip = skip,
                    Take = take,
                    TotalCount = totalCount
                };
        }

        public Task Create(TransactionAdded transaction)
        {
            DateTimeOffset createdAt = DateTimeOffset.UtcNow;

            string fromAssetIdColumn = transaction.FromAssetId != null ? "[FromAssetId]," : string.Empty;
            string fromAssetIdValue = transaction.FromAssetId != null ? $"'{transaction.FromAssetId}'," : string.Empty;
            string toAssetIdColumn = transaction.ToAssetId != null ? "[ToAssetId]," : string.Empty;
            string toAssetIdValue = transaction.ToAssetId != null ? $"'{transaction.ToAssetId}'," : string.Empty;
            string exchangeRateColumn = transaction.ExchangeRate != null ? "[ExchangeRate]," : string.Empty;
            string exchangeRateValue = transaction.ExchangeRate != null ? $"{transaction.ExchangeRate}," : string.Empty;


            var command = @$"Insert into {TransactionTableName}
                ([Id], [AssetId], [UserId], [TransactionType], [TransactionDate], [Amount], {fromAssetIdColumn} {toAssetIdColumn} {exchangeRateColumn} [Description], [CreatedAt] )
                    Values
                ('{transaction.Id}', '{transaction.AssetId}', '{transaction.UserId}', '{transaction.TransactionType}', '{transaction.TransactionDate}',
                '{transaction.Amount}',{fromAssetIdValue}{toAssetIdValue}{exchangeRateValue}'{transaction.Description}','{createdAt}')";

            return sqlDatabase.ExecuteNonQuery(command, CancellationToken.None);
        }

        public async Task Delete(string id)
        {
            var command = $"Delete from {TransactionTableName} where [Id] = '{id}'";

            await sqlDatabase.ExecuteNonQuery(command, CancellationToken.None);
        }
    }
}
