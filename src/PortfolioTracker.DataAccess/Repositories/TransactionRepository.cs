using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class TransactionRepository
    {
        public static readonly IList<TransactionAdded> Transactions;

        static TransactionRepository()
        {
            Transactions = new List<TransactionAdded>();
        }

        //TODO: delete after read model will be created
        public void AddTransaction(TransactionAdded transaction)
        {
            Transactions.Add(transaction);
        }

        public Task<PageResult<Transaction>> Get(string assetId, int skip, int take)
        {
            int totalCount = Transactions.Count(t => t.AssetId.Equals(assetId));

            List<Transaction> result = Transactions
                .Skip(skip)
                .Take(take)
                .Select(t => new Transaction
                {
                    Amount = t.Amount,
                    AssetId = t.AssetId,
                    CreatedAt = t.CreatedAt,
                    Description = t.Description,
                    ExchangeRate = t.ExchangeRate,
                    FromAssetId = t.FromAssetId,
                    Id = t.Id,
                    ToAssetId = t.ToAssetId,
                    TransactionDate = t.TransactionDate,
                    TransactionType = t.TransactionType,
                    UserId = t.UserId
                })
                .ToList();

            return Task.FromResult(
                new PageResult<Transaction>()
                {
                    Data = result,
                    Skip = skip,
                    Take = take,
                    TotalCount = totalCount
                });
        }
    }
}
