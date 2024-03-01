using PortfolioTracker.EventStore.Core;
using System.Collections.Generic;

namespace PortfolioTracker.Domain
{
    public class AggregateRoot
    {
        protected readonly List<IStoredEvent> EventsChanges = new List<IStoredEvent>();
        public IReadOnlyCollection<IStoredEvent> Changes => EventsChanges.AsReadOnly();

        protected void Apply(IStoredEvent e)
        {
            Mutate(e);
            EventsChanges.Add(e);
        }

        protected virtual void Mutate(IStoredEvent e)
        {
        }
    }
}
