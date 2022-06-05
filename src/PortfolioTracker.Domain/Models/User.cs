using System;

namespace PortfolioTracker.Domain.Models
{
    public class User
    {
        public string Id { get; }
        public string Name { get; set; }
        public Currency Currency { get; set; }

        public User(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
