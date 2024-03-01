using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PortfolioTracker.Domain.Models;
using PortfolioTracker.Events.Common;

namespace PortfolioTracker.DataAccess.Repositories
{
    public class UserRepository
    {
        private static readonly List<User> Users = new() { new() { Id = "testUser", MainCurrency = Currency.UAH, Name = "Roman Malyi" } };

        public Task<User> Get(string id)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Id.Equals(id)));
        }

        public Task<IEnumerable<User>> Get(int skip, int take)
        {
            return Task.FromResult(Users.Skip(skip).Take(take));
        }

        public Task<User> Create(string name, Currency currency)
        {
            User user = new() { Id = Guid.NewGuid().ToString(), Name = name, MainCurrency = currency };
            Users.Add(user);
            return Task.FromResult(user);
        }
    }
}
