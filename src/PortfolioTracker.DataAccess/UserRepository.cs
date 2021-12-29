using PortfolioTracker.Domain.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortfolioTracker.DataAccess
{
    public class UserRepository
    {
        private static List<User> users = new List<User>();

        public Task<User> Get(string id)
        {
            return Task.FromResult(users.FirstOrDefault(u => u.Id.Equals(id)));
        }

        public Task<IEnumerable<User>> Get(int skip, int take)
        {
            return Task.FromResult(users.Skip(skip).Take(take));
        }

        public Task<User> Create(string name)
        {
            var user = new User(name);
            users.Add(user);
            return Task.FromResult(user);
        }
    }
}
