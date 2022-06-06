using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioTracker.DataAccess.Models;
using PortfolioTracker.Domain.Models;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class AccountRepository
    {
        private static readonly List<Account> Accounts = new() { new Account { Id = "test", UserId = "testUser", AccountType = AccountType.Broker, Name = "FF" } };

        public Task<Account> Get(string id)
        {
            return Task.FromResult(Accounts.FirstOrDefault(u => u.Id.Equals(id)));
        }

        public Task<PageResult<Account>> Get(string userId, int skip, int take)
        {
            List<Account> result = Accounts.Where(a => a.UserId.Equals(userId, StringComparison.OrdinalIgnoreCase))
                .Skip(skip)
                .Take(take)
                .ToList();

            return Task.FromResult(
                new PageResult<Account>()
                {
                    Data = result,
                    Skip = skip,
                    Take = take,
                    TotalCount = Accounts.Count()
                });
        }

        public Task<Account> Create(string userId, string name, AccountType type)
        {
            Account account = new() { Id = Guid.NewGuid().ToString(), UserId = userId, AccountType = type, Name = name };
            Accounts.Add(account);
            return Task.FromResult(account);
        }
    }
}
