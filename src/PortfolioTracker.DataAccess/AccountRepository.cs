using PortfolioTracker.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess
{
    public class AccountRepository
    {
        private static List<Account> accounts = new List<Account>();

        public Task<Account> Get(string id)
        {
            return Task.FromResult(accounts.FirstOrDefault(u => u.Id.Equals(id)));
        }

        public Task<IEnumerable<Account>> Get(int skip, int take)
        {
            return Task.FromResult(accounts.Skip(skip).Take(take));
        }

        public Task<Account> Create(Account account)
        {
            accounts.Add(account);
            return Task.FromResult(account);
        }
    }
}
