using System;

namespace PortfolioTracker.Domain.Models
{
    public class User
    {
        public string Id { get; }
        public string Name { get; set; }
        //TODO: main currency should I have sql db?

        public User(string name)
        {
            Id = Guid.NewGuid().ToString();
            Name = name;
        }
    }
}
