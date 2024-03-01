using CSharpFunctionalExtensions;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class AccountRepository
    {
        private const string AccountTableName = "dbo.Account";

        private readonly SqlDatabase sqlDatabase;

        public AccountRepository(SqlDatabase sqlDatabase)
        {
            this.sqlDatabase = sqlDatabase;
        }

        public Task<Maybe<Account>> Get(string id)
        {
            var query = @$"Select * from {AccountTableName} where Id = '{id}'";
            return sqlDatabase.GetSingleOrDefault<Account>(query, CancellationToken.None);
        }

        public async Task<PageResult<Account>> Get(string userId, int skip, int take)
        {
            int totalCount = await sqlDatabase.ExecuteScalar<int>(@$"Select count(*) from {AccountTableName} where UserId = '{userId}'", CancellationToken.None);

            var query = @$"Select * from {AccountTableName} where UserId = '{userId}'
                            ORDER BY Id
                            OFFSET {skip} ROWS
                            FETCH NEXT {take} ROWS ONLY;";

            var result = await sqlDatabase.Query<Account>(query, CancellationToken.None);

            return new PageResult<Account>()
            {
                Data = result.ToList(),
                Skip = skip,
                Take = take,
                TotalCount = totalCount
            };
        }

        public async Task<Account> Create(string userId, string name, AccountType type)
        {
            string id = Guid.NewGuid().ToString();
            DateTimeOffset createdAt = DateTimeOffset.UtcNow;
            var command = @$"Insert into {AccountTableName}
                ([Id], [UserId], [Name], [AccountType], [CreatedAt] )
                    Values
                ('{id}', '{userId}', '{name}', '{type}', '{createdAt}')";

            await sqlDatabase.ExecuteNonQuery(command, CancellationToken.None);
            return new Account { Id = id, AccountType = type, CreatedAt = createdAt, Name = name, UserId = userId };
        }

        public async Task Delete(string id)
        {
            var command = $"Delete from {AccountTableName} where [Id] = '{id}'";

            await sqlDatabase.ExecuteNonQuery(command, CancellationToken.None);
        }
    }
}
