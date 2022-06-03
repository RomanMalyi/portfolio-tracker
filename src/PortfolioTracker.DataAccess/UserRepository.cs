using PortfolioTracker.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess
{
    public class UserRepository
    {
        private static readonly List<User> Users = new List<User>();

        public Task<User> Get(string id)
        {
            return Task.FromResult(Users.FirstOrDefault(u => u.Id.Equals(id)));
        }

        public Task<IEnumerable<User>> Get(int skip, int take)
        {
            return Task.FromResult(Users.Skip(skip).Take(take));
        }

        public Task<User> Create(string name)
        {
            var user = new User(name);
            Users.Add(user);
            return Task.FromResult(user);
        }
    }
}
