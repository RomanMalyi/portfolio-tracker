using System;

namespace PortfolioTracker.EventStore.Core
{
    public interface IStoredEvent
    {
        public DateTimeOffset CreatedAt { get; }
    }
}